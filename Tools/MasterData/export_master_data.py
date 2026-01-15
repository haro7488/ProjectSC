#!/usr/bin/env python3
"""
Master Data Export Script
Excel → JSON 변환 및 검증 스크립트

사용법:
    python export_master_data.py                    # Excel에서 JSON 변환
    python export_master_data.py --validate-only    # JSON 검증만
    python export_master_data.py --source json      # JSON 파일 검증 및 재출력

필요 패키지:
    pip install pandas openpyxl
"""

import json
import os
import sys
from datetime import datetime
from pathlib import Path
from typing import Any

# 프로젝트 경로 설정
SCRIPT_DIR = Path(__file__).parent
PROJECT_ROOT = SCRIPT_DIR.parent.parent
OUTPUT_DIR = PROJECT_ROOT / "Assets" / "Data" / "MasterData"
EXCEL_PATH = SCRIPT_DIR / "MasterData.xlsx"

# Enum 정의
ENUMS = {
    "Rarity": ["N", "R", "SR", "SSR"],
    "CharacterClass": ["Warrior", "Mage", "Assassin", "Healer", "Tank", "Archer"],
    "Element": ["Fire", "Water", "Dark", "Light", "Earth", "Wind"],
    "SkillType": ["Active", "Passive", "Ultimate"],
    "TargetType": ["SingleEnemy", "AllEnemy", "SingleAlly", "AllAlly", "Self"],
    "ItemType": ["Weapon", "Armor", "Accessory", "Consumable"],
    "Difficulty": ["Easy", "Normal", "Hard"],
    "GachaType": ["Standard", "Pickup", "Free"],
    "CostType": ["Gold", "Gem", "Ticket", "None"],
}

# 시트별 스키마 정의
SCHEMAS = {
    "Character": {
        "required": ["Id", "Name", "NameEn", "Rarity", "Class", "Element",
                     "BaseHP", "BaseATK", "BaseDEF", "BaseSPD", "CritRate", "CritDamage", "SkillIds"],
        "types": {
            "Id": str, "Name": str, "NameEn": str,
            "Rarity": ("enum", "Rarity"),
            "Class": ("enum", "CharacterClass"),
            "Element": ("enum", "Element"),
            "BaseHP": int, "BaseATK": int, "BaseDEF": int, "BaseSPD": int,
            "CritRate": float, "CritDamage": float,
            "SkillIds": list,
            "Description": str,
        },
        "id_prefix": "char_",
    },
    "Skill": {
        "required": ["Id", "Name", "NameEn", "Type", "TargetType", "Element", "Power", "CoolDown", "ManaCost"],
        "types": {
            "Id": str, "Name": str, "NameEn": str,
            "Type": ("enum", "SkillType"),
            "TargetType": ("enum", "TargetType"),
            "Element": ("enum", "Element"),
            "Power": int, "CoolDown": int, "ManaCost": int,
            "Description": str,
        },
        "id_prefix": "skill_",
    },
    "Item": {
        "required": ["Id", "Name", "NameEn", "Type", "Rarity", "ATKBonus", "DEFBonus", "HPBonus"],
        "types": {
            "Id": str, "Name": str, "NameEn": str,
            "Type": ("enum", "ItemType"),
            "Rarity": ("enum", "Rarity"),
            "ATKBonus": int, "DEFBonus": int, "HPBonus": int,
            "Description": str,
        },
        "id_prefix": "item_",
    },
    "Stage": {
        "required": ["Id", "Name", "NameEn", "Chapter", "StageNumber", "Difficulty",
                     "StaminaCost", "RecommendedPower", "EnemyIds", "RewardGold", "RewardExp"],
        "types": {
            "Id": str, "Name": str, "NameEn": str,
            "Chapter": int, "StageNumber": int,
            "Difficulty": ("enum", "Difficulty"),
            "StaminaCost": int, "RecommendedPower": int,
            "EnemyIds": list,
            "RewardGold": int, "RewardExp": int,
            "RewardItemIds": list, "RewardItemRates": list,
            "Description": str,
        },
        "id_prefix": "stage_",
    },
    "GachaPool": {
        "required": ["Id", "Name", "NameEn", "Type", "CostType", "CostAmount",
                     "CostAmount10", "PityCount", "CharacterIds", "Rates", "IsActive"],
        "types": {
            "Id": str, "Name": str, "NameEn": str,
            "Type": ("enum", "GachaType"),
            "CostType": ("enum", "CostType"),
            "CostAmount": int, "CostAmount10": int, "PityCount": int,
            "CharacterIds": list,
            "Rates": dict,
            "RateUpCharacterId": str, "RateUpBonus": float,
            "IsActive": bool,
            "StartDate": str, "EndDate": str,
            "Description": str,
        },
        "id_prefix": "gacha_",
    },
}


class ValidationError(Exception):
    """데이터 검증 오류"""
    pass


class MasterDataExporter:
    """마스터 데이터 변환/검증 클래스"""

    def __init__(self):
        self.errors: list[str] = []
        self.warnings: list[str] = []
        self.all_ids: dict[str, set] = {}  # 참조 무결성 검증용

    def log_error(self, sheet: str, row: int | None, message: str):
        """에러 로그"""
        if row is not None:
            self.errors.append(f"[{sheet}:Row {row}] {message}")
        else:
            self.errors.append(f"[{sheet}] {message}")

    def log_warning(self, sheet: str, row: int | None, message: str):
        """경고 로그"""
        if row is not None:
            self.warnings.append(f"[{sheet}:Row {row}] {message}")
        else:
            self.warnings.append(f"[{sheet}] {message}")

    def validate_enum(self, value: str, enum_name: str) -> bool:
        """Enum 값 검증"""
        return value in ENUMS.get(enum_name, [])

    def validate_type(self, value: Any, expected_type: Any, field: str, sheet: str, row: int) -> bool:
        """타입 검증"""
        if value is None:
            return True  # None은 optional 필드에서 허용

        if isinstance(expected_type, tuple) and expected_type[0] == "enum":
            if not self.validate_enum(value, expected_type[1]):
                self.log_error(sheet, row, f"{field}: '{value}'는 {expected_type[1]} Enum에 없음")
                return False
        elif expected_type == int:
            if not isinstance(value, int) or isinstance(value, bool):
                self.log_error(sheet, row, f"{field}: int 타입 필요, {type(value).__name__} 받음")
                return False
        elif expected_type == float:
            if not isinstance(value, (int, float)) or isinstance(value, bool):
                self.log_error(sheet, row, f"{field}: float 타입 필요, {type(value).__name__} 받음")
                return False
        elif expected_type == str:
            if not isinstance(value, str):
                self.log_error(sheet, row, f"{field}: str 타입 필요, {type(value).__name__} 받음")
                return False
        elif expected_type == list:
            if not isinstance(value, list):
                self.log_error(sheet, row, f"{field}: list 타입 필요, {type(value).__name__} 받음")
                return False
        elif expected_type == dict:
            if not isinstance(value, dict):
                self.log_error(sheet, row, f"{field}: dict 타입 필요, {type(value).__name__} 받음")
                return False
        elif expected_type == bool:
            if not isinstance(value, bool):
                self.log_error(sheet, row, f"{field}: bool 타입 필요, {type(value).__name__} 받음")
                return False

        return True

    def validate_record(self, record: dict, schema: dict, sheet: str, row: int) -> bool:
        """레코드 검증"""
        is_valid = True

        # 필수 필드 검증
        for field in schema["required"]:
            if field not in record or record[field] is None:
                self.log_error(sheet, row, f"필수 필드 '{field}' 누락")
                is_valid = False

        # 타입 검증
        for field, expected_type in schema["types"].items():
            if field in record:
                if not self.validate_type(record[field], expected_type, field, sheet, row):
                    is_valid = False

        # ID 형식 검증
        if "Id" in record and "id_prefix" in schema:
            id_value = record["Id"]
            if not id_value.startswith(schema["id_prefix"]):
                self.log_warning(sheet, row, f"ID '{id_value}'가 권장 접두사 '{schema['id_prefix']}'로 시작하지 않음")

        return is_valid

    def validate_references(self, data: dict[str, list]) -> bool:
        """참조 무결성 검증"""
        is_valid = True

        # 모든 ID 수집
        for sheet_name, records in data.items():
            self.all_ids[sheet_name] = {r["Id"] for r in records if "Id" in r}

        # Character의 SkillIds 참조 검증
        if "Character" in data and "Skill" in self.all_ids:
            for row, char in enumerate(data["Character"], start=2):
                for skill_id in char.get("SkillIds", []):
                    if skill_id not in self.all_ids["Skill"]:
                        self.log_error("Character", row, f"SkillId '{skill_id}' 참조 실패 - Skill에 없음")
                        is_valid = False

        # GachaPool의 CharacterIds 참조 검증
        if "GachaPool" in data and "Character" in self.all_ids:
            for row, gacha in enumerate(data["GachaPool"], start=2):
                for char_id in gacha.get("CharacterIds", []):
                    if char_id not in self.all_ids["Character"]:
                        self.log_error("GachaPool", row, f"CharacterId '{char_id}' 참조 실패 - Character에 없음")
                        is_valid = False
                # RateUpCharacterId 검증
                rate_up = gacha.get("RateUpCharacterId")
                if rate_up and rate_up not in self.all_ids["Character"]:
                    self.log_error("GachaPool", row, f"RateUpCharacterId '{rate_up}' 참조 실패")
                    is_valid = False

        # Stage의 RewardItemIds 참조 검증
        if "Stage" in data and "Item" in self.all_ids:
            for row, stage in enumerate(data["Stage"], start=2):
                for item_id in stage.get("RewardItemIds", []):
                    if item_id not in self.all_ids["Item"]:
                        self.log_error("Stage", row, f"RewardItemId '{item_id}' 참조 실패 - Item에 없음")
                        is_valid = False

        # GachaPool Rates 합계 검증
        if "GachaPool" in data:
            for row, gacha in enumerate(data["GachaPool"], start=2):
                rates = gacha.get("Rates", {})
                total = sum(rates.values())
                if abs(total - 1.0) > 0.001:
                    self.log_error("GachaPool", row, f"Rates 합계가 1.0이 아님: {total}")
                    is_valid = False

        return is_valid

    def load_from_json(self) -> dict[str, list]:
        """JSON 파일에서 데이터 로드"""
        data = {}

        for sheet_name in SCHEMAS.keys():
            json_path = OUTPUT_DIR / f"{sheet_name}.json"
            if json_path.exists():
                with open(json_path, "r", encoding="utf-8") as f:
                    content = json.load(f)
                    data[sheet_name] = content.get("data", [])
                print(f"  ✓ {sheet_name}.json 로드 ({len(data[sheet_name])}개)")
            else:
                print(f"  ⚠ {sheet_name}.json 없음 (건너뜀)")

        return data

    def load_from_excel(self) -> dict[str, list]:
        """Excel 파일에서 데이터 로드"""
        try:
            import pandas as pd
        except ImportError:
            print("❌ pandas 패키지가 필요합니다: pip install pandas openpyxl")
            sys.exit(1)

        if not EXCEL_PATH.exists():
            print(f"❌ Excel 파일 없음: {EXCEL_PATH}")
            sys.exit(1)

        data = {}
        xl = pd.ExcelFile(EXCEL_PATH)

        for sheet_name in SCHEMAS.keys():
            if sheet_name in xl.sheet_names:
                df = pd.read_excel(xl, sheet_name=sheet_name)
                # NaN을 None으로 변환
                df = df.where(pd.notnull(df), None)

                # 배열 필드 처리 (콤마 구분 문자열 → 리스트)
                records = []
                for _, row in df.iterrows():
                    record = row.to_dict()
                    # 콤마 구분 문자열을 리스트로 변환
                    for field, field_type in SCHEMAS[sheet_name]["types"].items():
                        if field_type == list and field in record and isinstance(record[field], str):
                            record[field] = [x.strip() for x in record[field].split(",") if x.strip()]
                    records.append(record)

                data[sheet_name] = records
                print(f"  ✓ {sheet_name} 시트 로드 ({len(records)}개)")
            else:
                print(f"  ⚠ {sheet_name} 시트 없음 (건너뜀)")

        return data

    def validate_all(self, data: dict[str, list]) -> bool:
        """전체 데이터 검증"""
        print("\n📋 데이터 검증 중...")
        is_valid = True

        # 개별 레코드 검증
        for sheet_name, records in data.items():
            if sheet_name not in SCHEMAS:
                continue
            schema = SCHEMAS[sheet_name]
            for row, record in enumerate(records, start=2):
                if not self.validate_record(record, schema, sheet_name, row):
                    is_valid = False

        # 참조 무결성 검증
        if not self.validate_references(data):
            is_valid = False

        # 결과 출력
        if self.errors:
            print(f"\n❌ 에러 {len(self.errors)}개:")
            for err in self.errors[:20]:  # 최대 20개만 출력
                print(f"  {err}")
            if len(self.errors) > 20:
                print(f"  ... 외 {len(self.errors) - 20}개")

        if self.warnings:
            print(f"\n⚠️ 경고 {len(self.warnings)}개:")
            for warn in self.warnings[:10]:
                print(f"  {warn}")
            if len(self.warnings) > 10:
                print(f"  ... 외 {len(self.warnings) - 10}개")

        if is_valid and not self.errors:
            print("\n✅ 검증 완료 - 에러 없음")

        return is_valid and not self.errors

    def export_to_json(self, data: dict[str, list]):
        """JSON 파일로 출력"""
        print("\n📦 JSON 출력 중...")
        OUTPUT_DIR.mkdir(parents=True, exist_ok=True)

        timestamp = datetime.utcnow().strftime("%Y-%m-%dT%H:%M:%SZ")

        for sheet_name, records in data.items():
            output = {
                "version": "1.0.0",
                "exportedAt": timestamp,
                "data": records,
            }

            output_path = OUTPUT_DIR / f"{sheet_name}.json"
            with open(output_path, "w", encoding="utf-8") as f:
                json.dump(output, f, ensure_ascii=False, indent=2)

            print(f"  ✓ {output_path.name} ({len(records)}개)")

        print(f"\n✅ 출력 완료: {OUTPUT_DIR}")


def main():
    import argparse

    parser = argparse.ArgumentParser(description="Master Data Export Script")
    parser.add_argument("--source", choices=["excel", "json"], default="json",
                        help="데이터 소스 (기본: json)")
    parser.add_argument("--validate-only", action="store_true",
                        help="검증만 수행 (출력 안 함)")
    args = parser.parse_args()

    print("=" * 50)
    print("Master Data Export Tool")
    print("=" * 50)

    exporter = MasterDataExporter()

    # 데이터 로드
    print(f"\n📂 데이터 로드 중 (소스: {args.source})...")
    if args.source == "excel":
        data = exporter.load_from_excel()
    else:
        data = exporter.load_from_json()

    if not data:
        print("❌ 로드된 데이터 없음")
        sys.exit(1)

    # 검증
    is_valid = exporter.validate_all(data)

    # 출력
    if not args.validate_only and is_valid:
        exporter.export_to_json(data)
    elif not is_valid:
        print("\n❌ 검증 실패 - 출력 건너뜀")
        sys.exit(1)

    print("\n" + "=" * 50)


if __name__ == "__main__":
    main()
