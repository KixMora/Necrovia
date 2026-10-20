"""Собирает все .cs из Assets/Scripts в один Word-файл (Courier New 9 pt для кода).
Запуск: python export_scripts_to_word.py
Требуется: pip install python-docx
"""
import os
import re
from pathlib import Path

from docx import Document
from docx.enum.text import WD_BREAK, WD_LINE_SPACING
from docx.oxml.ns import qn
from docx.shared import Inches, Pt

REPO = Path(__file__).resolve().parent
SCRIPTS_ROOT = REPO / "Assets" / "Scripts"
OUTPUT = REPO / "Necrovia_Assets_Scripts_Code.docx"

DESCRIPTIONS = {
    "Player/PlayerShooting.cs": "Ввод и логика стрельбы игрока (очередь, перезарядка, смена режима огня).",
    "Weapons/BulletTracer.cs": "Визуальный след/трассер выстрела.",
    "Weapons/WeaponVisualController.cs": "Отображение модели оружия у персонажа.",
    "Weapons/WeaponAnimationController.cs": "Анимации оружия (отдача, смена и т.п.).",
    "Weapons/WeaponData.cs": "ScriptableObject и перечисления: данные и режимы оружия.",
    "Transition/BootstrapLoader.cs": "Стартовая загрузка/инициализация при входе в игру.",
    "UI/AmmoUI.cs": "Интерфейс патронов и магазина.",
    "Weapons/PlayerWeaponInventory.cs": "Инвентарь и экипировка оружия у игрока.",
    "Player/PlayerMovement.cs": "Движение персонажа (клавиатура/джойстик).",
    "Player/MobileJoystick.cs": "Виртуальный джойстик для тач-ввода.",
    "Player/PlayerHealth.cs": "Здоровье, урон, смерть игрока.",
    "UI/DamageBloodOverlay.cs": "Красное затемнение экрана при получении урона.",
    "Mission/MissionResultUI.cs": "Экран итога миссии (победа/поражение).",
    "Mission/MissionRuntimeTracker.cs": "Счётчики и прогресс целей миссии во время игры.",
    "Mission/MissionEndType.cs": "Перечисление: как завершилась миссия.",
    "Mission/MissionMapUI.cs": "Карта миссий и узлы на UI.",
    "Mission/MissionData.cs": "ScriptableObject: описание одной миссии.",
    "Mission/MissionObjectivesUI.cs": "Список целей миссии в HUD.",
    "UI/WaveUI.cs": "Отображение номера волны и связанной информации.",
    "UI/ObjectivesPanelToggle.cs": "Показ/скрытие панели целей.",
    "Transition/SceneTransitionManager.cs": "Асинхронные переходы между сценами.",
    "UI/ButtonGlowPulse.cs": "Пульсирующее свечение кнопки (UI-декор).",
    "Analytics/GameAnalyticsInitializer.cs": "Инициализация SDK GameAnalytics.",
    "Combat/WaveManager.cs": "Спавн волн врагов и логика волн.",
    "Analytics/AnalyticsManager.cs": "Обёртка отправки игровых событий в аналитику.",
    "Mission/MissionConnectionLineUI.cs": "Линии связи между узлами на карте миссий.",
    "Mission/MissionNodeGlow.cs": "Подсветка/свечение узла карты.",
    "Mission/MissionNodeUI.cs": "Один узел (кнопка/иконка) на карте миссий.",
    "Mission/MissionManager.cs": "Центральное управление текущей миссией и целями.",
    "Mission/MissionObjectiveLineUI.cs": "Линии от узла к целям на карте.",
    "Mission/Editor/MissionPointSyncTool.cs": "Редакторская утилита: синхронизация точек миссии.",
    "Mission/MissionPoint.cs": "Точка на сцене, привязанная к миссии/карте.",
    "Systems/MusicManager.cs": "Фоновая музыка и переключение треков.",
    "Enemies/ZombieAI.cs": "Поведение и преследование зомби.",
    "Mission/MissionObjective.cs": "Сериализуемая цель миссии (данные + состояние).",
    "Mission/MissionObjectiveType.cs": "Тип цели миссии (перечисление).",
    "Mission/MissionCompletionMode.cs": "Режим завершения миссии (перечисление).",
    "Mission/MissionSelectButtonUI.cs": "Кнопка выбора конкретной миссии в меню.",
    "Mission/MissionGameplayLoader.cs": "Загрузка игровой сцены после выбора миссии.",
    "UI/MainMenuUI.cs": "Главное меню: кнопки, навигация.",
    "Mission/MissionSelectUI.cs": "Экран выбора миссии.",
    "Transition/ScreenFader.cs": "Затемнение/осветление экрана при переходах.",
    "Systems/LanguageManager.cs": "Язык интерфейса и строки локализации.",
    "Enemies/ZombieSpawner.cs": "Спавн зомби по правилам/точкам.",
    "Enemies/ZombieGroupController.cs": "Координация группы зомби как одного объекта.",
    "Enemies/ZombieGroupVisual.cs": "Визуал группы зомби на сцене.",
    "UI/PlayerHealthUI.cs": "Полоска/цифры здоровья игрока.",
    "Enemies/ZombieAnimator.cs": "Связь состояния зомби с Animator.",
    "Systems/CameraFollow.cs": "Камера следует за целью (игрок).",
    "Combat/TracerMover.cs": "Движение визуала трассера по траектории.",
}


def set_times_new_roman(run, size_pt=12, bold=False, italic=False):
    run.font.name = "Times New Roman"
    run.font.size = Pt(size_pt)
    run.bold = bold
    run.italic = italic
    rfonts = run._element.rPr.rFonts
    if rfonts is not None:
        for q in ("w:ascii", "w:hAnsi", "w:cs", "w:eastAsia"):
            rfonts.set(qn(q), "Times New Roman")


def set_courier(run, size_pt=9):
    run.font.name = "Courier New"
    run.font.size = Pt(size_pt)
    rfonts = run._element.rPr.rFonts
    if rfonts is not None:
        rfonts.set(qn("w:ascii"), "Courier New")
        rfonts.set(qn("w:hAnsi"), "Courier New")
        rfonts.set(qn("w:cs"), "Courier New")
        rfonts.set(qn("w:eastAsia"), "Courier New")


def compact_blank_lines(text: str) -> str:
    """Схлопывает цепочки пустых строк в исходнике до одного перевода строки (без «двойных» пропусков)."""
    text = text.replace("\r\n", "\n")
    text = re.sub(r"\n(?:[ \t]*\n)+", "\n", text)
    return text.strip("\n")


def tighten_paragraph(p):
    pf = p.paragraph_format
    pf.space_before = Pt(0)
    pf.space_after = Pt(0)
    pf.line_spacing_rule = WD_LINE_SPACING.MULTIPLE
    pf.line_spacing = 1.0


def add_code_block(doc, text: str):
    text = compact_blank_lines(text)
    p = doc.add_paragraph()
    tighten_paragraph(p)
    # Плотнее: одинарный интервал для моноширинного блока
    p.paragraph_format.line_spacing_rule = WD_LINE_SPACING.EXACTLY
    p.paragraph_format.line_spacing = Pt(10.5)
    lines = text.splitlines()
    if not lines:
        r = p.add_run(" ")
        set_courier(r)
        return
    for i, line in enumerate(lines):
        if i:
            p.add_run().add_break(WD_BREAK.LINE)
        r = p.add_run(line if line else " ")
        set_courier(r)


def collect_cs_files(root: Path):
    out = []
    for dirpath, dirnames, filenames in os.walk(root):
        dirnames[:] = sorted(d for d in dirnames if d not in (".git",))
        for name in sorted(filenames):
            if name.endswith(".cs"):
                out.append(Path(dirpath) / name)
    return sorted(out, key=lambda p: str(p).lower())


def main():
    doc = Document()
    sec = doc.sections[0]
    sec.top_margin = Inches(0.5)
    sec.bottom_margin = Inches(0.5)
    sec.left_margin = Inches(0.5)
    sec.right_margin = Inches(0.5)

    h = doc.add_paragraph()
    tighten_paragraph(h)
    hr = h.add_run("Necrovia — код C# (Assets/Scripts)")
    set_times_new_roman(hr, 12, bold=True, italic=False)

    sub = doc.add_paragraph()
    tighten_paragraph(sub)
    sr = sub.add_run("Код: Courier New 9 pt. Путь и пометка — Times New Roman 12.")
    set_times_new_roman(sr, 12, bold=False, italic=False)

    files = collect_cs_files(SCRIPTS_ROOT)
    for path in files:
        rel = path.relative_to(SCRIPTS_ROOT).as_posix()
        desc_text = DESCRIPTIONS.get(rel, "Скрипт Unity (C#).")

        head = doc.add_paragraph()
        tighten_paragraph(head)
        r_path = head.add_run(rel + " — ")
        set_times_new_roman(r_path, 12, bold=True, italic=False)
        r_desc = head.add_run(desc_text)
        set_times_new_roman(r_desc, 12, bold=False, italic=True)

        body = path.read_text(encoding="utf-8", errors="replace")
        add_code_block(doc, body)

    doc.save(str(OUTPUT))
    print("Saved:", OUTPUT)


if __name__ == "__main__":
    main()
