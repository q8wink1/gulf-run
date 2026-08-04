#!/usr/bin/env python3
"""Patch Gameplay.unity for Sprint 23.3 / 23.4 / 23.5 (no Unity Editor required).

Modes:
  --hud      Sprint 23.3 Gameplay HUD canvas + disable overlapping OnGUI race HUD
  --player   Sprint 23.4 Runner player + ground plane
  --camera   Sprint 23.5 RunnerCameraFollow + CameraShake stubs
  --all      All of the above (default)

Sprint 23.6 endless track: run `_tools/patch_gameplay_sprint_23_6_track.py` separately.
"""

from __future__ import annotations

import argparse
import re
from dataclasses import dataclass, field
from pathlib import Path

SCENE = Path(r"C:\Projects\GulfRun\Client\Assets\_Project\Scenes\Gameplay.unity")

GUID_TEXT = "5f7201a12d95ffc409449d95f23cf332"
GUID_IMAGE = "fe87c0e1cc204ed48ad3b37840f39efc"
GUID_BUTTON = "4e29b1a8efbd4b44bb3f3716e73f07ff"
GUID_SCALER = "0cd44c1031e13a943bb63640046fad76"
GUID_RAYCASTER = "dc42784cf147c0c48a680349fa168899"
GUID_SHADOW = "cfabb0440166ab443bba8876756fdfa9"
GUID_EVENTSYSTEM = "76c392e42b5098c458856cdf6ecaaaa1"
GUID_STANDALONE = "4f231c4fb786f3946a6b90b886c48677"
GUID_HUD = "b20c00000000000000000000000000b1"
GUID_RUNNER = "b20c00000000000000000000000000b2"
GUID_SWIPE = "b20c00000000000000000000000000b3"
GUID_ANIM = "b20c00000000000000000000000000b5"
GUID_CAM_FOLLOW = "b20c00000000000000000000000000b7"
GUID_CAM_SHAKE = "b20c00000000000000000000000000b8"
GUID_CAM_FX = "b20c00000000000000000000000000b9"
GUID_CONFIG = "b20c00000000000000000000000000bb"

FONT = "{fileID: 10102, guid: 0000000000000000e000000000000000, type: 0}"
CUBE_MESH = "{fileID: 10202, guid: 0000000000000000e000000000000000, type: 0}"
CAPSULE_MESH = "{fileID: 10208, guid: 0000000000000000e000000000000000, type: 0}"
LIT_MAT = "{fileID: 10303, guid: 0000000000000000f000000000000000, type: 0}"

GOLD = (0.90, 0.71, 0.25, 1.0)
GOLD_BRIGHT = (1.0, 0.84, 0.40, 1.0)
PANEL_BG = (0.10, 0.09, 0.10, 0.78)
PANEL_BORDER = (0.90, 0.71, 0.25, 0.55)
WHITE = (1.0, 1.0, 1.0, 1.0)
MUTED = (0.80, 0.80, 0.80, 1.0)
BUTTON_DARK = (0.12, 0.10, 0.09, 0.92)
GOLD_LABEL = (0.20, 0.14, 0.02, 1.0)
BOOST_FILL = (0.35, 0.75, 1.0, 1.0)
DIM = (0.02, 0.02, 0.04, 0.72)
GEM = (0.55, 0.85, 1.0, 1.0)
SUCCESS = (0.40, 0.85, 0.45, 1.0)

_next = 510000000


def nid() -> int:
    global _next
    _next += 1
    return _next


def c4(t: tuple[float, float, float, float]) -> str:
    return f"{{r: {t[0]}, g: {t[1]}, b: {t[2]}, a: {t[3]}}}"


@dataclass
class Node:
    name: str
    go: int = field(default_factory=nid)
    rt: int = field(default_factory=nid)
    active: int = 1
    children: list[Node] = field(default_factory=list)
    amin: tuple[float, float] = (0.5, 0.5)
    amax: tuple[float, float] = (0.5, 0.5)
    pos: tuple[float, float] = (0.0, 0.0)
    size: tuple[float, float] = (100.0, 100.0)
    pivot: tuple[float, float] = (0.5, 0.5)
    image: tuple[float, float, float, float] | None = None
    raycast: int = 0
    shadow: bool = False
    text: str | None = None
    font_size: int = 24
    font_style: int = 1
    align: int = 4
    text_color: tuple[float, float, float, float] = WHITE
    button: bool = False
    filled: bool = False
    fill_amount: float = 1.0
    cr: int = field(default_factory=nid)
    img_id: int | None = None
    txt_id: int | None = None
    btn_id: int | None = None
    sh_id: int | None = None

    def prep(self) -> None:
        if self.image is not None or self.button:
            self.img_id = nid()
        if self.text is not None:
            self.txt_id = nid()
        if self.button:
            self.btn_id = nid()
        if self.shadow:
            self.sh_id = nid()
        for ch in self.children:
            ch.prep()


def comps(n: Node) -> list[int]:
    out = [n.rt]
    needs_graphic = n.image is not None or n.text is not None or n.button
    if needs_graphic:
        out.append(n.cr)
    if n.img_id:
        out.append(n.img_id)
    if n.txt_id:
        out.append(n.txt_id)
    if n.btn_id:
        out.append(n.btn_id)
    if n.sh_id:
        out.append(n.sh_id)
    return out


def emit_node(lines: list[str], n: Node, father: int) -> None:
    lines.append(f"--- !u!1 &{n.go}")
    lines.append("GameObject:")
    lines.append("  m_ObjectHideFlags: 0")
    lines.append("  m_CorrespondingSourceObject: {fileID: 0}")
    lines.append("  m_PrefabInstance: {fileID: 0}")
    lines.append("  m_PrefabAsset: {fileID: 0}")
    lines.append("  serializedVersion: 6")
    lines.append("  m_Component:")
    for c in comps(n):
        lines.append(f"  - component: {{fileID: {c}}}")
    lines.append("  m_Layer: 5")
    lines.append(f"  m_Name: {n.name}")
    lines.append("  m_TagString: Untagged")
    lines.append("  m_Icon: {fileID: 0}")
    lines.append("  m_NavMeshLayer: 0")
    lines.append("  m_StaticEditorFlags: 0")
    lines.append(f"  m_IsActive: {n.active}")
    lines.append(f"--- !u!224 &{n.rt}")
    lines.append("RectTransform:")
    lines.append("  m_ObjectHideFlags: 0")
    lines.append("  m_CorrespondingSourceObject: {fileID: 0}")
    lines.append("  m_PrefabInstance: {fileID: 0}")
    lines.append("  m_PrefabAsset: {fileID: 0}")
    lines.append(f"  m_GameObject: {{fileID: {n.go}}}")
    lines.append("  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}")
    lines.append("  m_LocalPosition: {x: 0, y: 0, z: 0}")
    lines.append("  m_LocalScale: {x: 1, y: 1, z: 1}")
    lines.append("  m_ConstrainProportionsScale: 0")
    lines.append("  m_Children:")
    for ch in n.children:
        lines.append(f"  - {{fileID: {ch.rt}}}")
    lines.append(f"  m_Father: {{fileID: {father}}}")
    lines.append("  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}")
    lines.append(f"  m_AnchorMin: {{x: {n.amin[0]}, y: {n.amin[1]}}}")
    lines.append(f"  m_AnchorMax: {{x: {n.amax[0]}, y: {n.amax[1]}}}")
    lines.append(f"  m_AnchoredPosition: {{x: {n.pos[0]}, y: {n.pos[1]}}}")
    lines.append(f"  m_SizeDelta: {{x: {n.size[0]}, y: {n.size[1]}}}")
    lines.append(f"  m_Pivot: {{x: {n.pivot[0]}, y: {n.pivot[1]}}}")

    needs_graphic = n.image is not None or n.text is not None or n.button
    if needs_graphic:
        lines += [
            f"--- !u!222 &{n.cr}",
            "CanvasRenderer:",
            "  m_ObjectHideFlags: 0",
            "  m_CorrespondingSourceObject: {fileID: 0}",
            "  m_PrefabInstance: {fileID: 0}",
            "  m_PrefabAsset: {fileID: 0}",
            f"  m_GameObject: {{fileID: {n.go}}}",
            "  m_CullTransparentMesh: 1",
        ]

    if n.img_id is not None:
        img_type = 3 if n.filled else 0
        lines += [
            f"--- !u!114 &{n.img_id}",
            "MonoBehaviour:",
            "  m_ObjectHideFlags: 0",
            "  m_CorrespondingSourceObject: {fileID: 0}",
            "  m_PrefabInstance: {fileID: 0}",
            "  m_PrefabAsset: {fileID: 0}",
            f"  m_GameObject: {{fileID: {n.go}}}",
            "  m_Enabled: 1",
            "  m_EditorHideFlags: 0",
            f"  m_Script: {{fileID: 11500000, guid: {GUID_IMAGE}, type: 3}}",
            "  m_Name: ",
            "  m_EditorClassIdentifier: ",
            "  m_Material: {fileID: 0}",
            f"  m_Color: {c4(n.image if n.image is not None else WHITE)}",
            f"  m_RaycastTarget: {n.raycast}",
            "  m_RaycastPadding: {x: 0, y: 0, z: 0, w: 0}",
            "  m_Maskable: 1",
            "  m_OnCullStateChanged:",
            "    m_PersistentCalls:",
            "      m_Calls: []",
            "  m_Sprite: {fileID: 0}",
            f"  m_Type: {img_type}",
            "  m_PreserveAspect: 0",
            "  m_FillCenter: 1",
            "  m_FillMethod: 0",
            f"  m_FillAmount: {n.fill_amount}",
            "  m_FillClockwise: 1",
            "  m_FillOrigin: 0",
            "  m_UseSpriteMesh: 0",
            "  m_PixelsPerUnitMultiplier: 1",
        ]

    if n.txt_id is not None:
        lines += [
            f"--- !u!114 &{n.txt_id}",
            "MonoBehaviour:",
            "  m_ObjectHideFlags: 0",
            "  m_CorrespondingSourceObject: {fileID: 0}",
            "  m_PrefabInstance: {fileID: 0}",
            "  m_PrefabAsset: {fileID: 0}",
            f"  m_GameObject: {{fileID: {n.go}}}",
            "  m_Enabled: 1",
            "  m_EditorHideFlags: 0",
            f"  m_Script: {{fileID: 11500000, guid: {GUID_TEXT}, type: 3}}",
            "  m_Name: ",
            "  m_EditorClassIdentifier: ",
            "  m_Material: {fileID: 0}",
            f"  m_Color: {c4(n.text_color)}",
            "  m_RaycastTarget: 0",
            "  m_RaycastPadding: {x: 0, y: 0, z: 0, w: 0}",
            "  m_Maskable: 1",
            "  m_OnCullStateChanged:",
            "    m_PersistentCalls:",
            "      m_Calls: []",
            "  m_FontData:",
            "    m_Font: " + FONT,
            f"    m_FontSize: {n.font_size}",
            f"    m_FontStyle: {n.font_style}",
            "    m_BestFit: 0",
            "    m_MinSize: 10",
            "    m_MaxSize: 72",
            f"    m_Alignment: {n.align}",
            "    m_AlignByGeometry: 0",
            "    m_RichText: 1",
            "    m_HorizontalOverflow: 0",
            "    m_VerticalOverflow: 0",
            "    m_LineSpacing: 1",
            f"  m_Text: {n.text}",
        ]

    if n.btn_id is not None:
        lines += [
            f"--- !u!114 &{n.btn_id}",
            "MonoBehaviour:",
            "  m_ObjectHideFlags: 0",
            "  m_CorrespondingSourceObject: {fileID: 0}",
            "  m_PrefabInstance: {fileID: 0}",
            "  m_PrefabAsset: {fileID: 0}",
            f"  m_GameObject: {{fileID: {n.go}}}",
            "  m_Enabled: 1",
            "  m_EditorHideFlags: 0",
            f"  m_Script: {{fileID: 11500000, guid: {GUID_BUTTON}, type: 3}}",
            "  m_Name: ",
            "  m_EditorClassIdentifier: ",
            "  m_Navigation:",
            "    m_Mode: 3",
            "    m_WrapAround: 0",
            "    m_SelectOnUp: {fileID: 0}",
            "    m_SelectOnDown: {fileID: 0}",
            "    m_SelectOnLeft: {fileID: 0}",
            "    m_SelectOnRight: {fileID: 0}",
            "  m_Transition: 1",
            "  m_Colors:",
            "    m_NormalColor: {r: 1, g: 1, b: 1, a: 1}",
            "    m_HighlightedColor: {r: 0.96, g: 0.96, b: 0.96, a: 1}",
            "    m_PressedColor: {r: 0.78, g: 0.78, b: 0.78, a: 1}",
            "    m_SelectedColor: {r: 0.96, g: 0.96, b: 0.96, a: 1}",
            "    m_DisabledColor: {r: 0.78, g: 0.78, b: 0.78, a: 0.5}",
            "    m_ColorMultiplier: 1",
            "    m_FadeDuration: 0.1",
            "  m_SpriteState:",
            "    m_HighlightedSprite: {fileID: 0}",
            "    m_PressedSprite: {fileID: 0}",
            "    m_SelectedSprite: {fileID: 0}",
            "    m_DisabledSprite: {fileID: 0}",
            "  m_AnimationTriggers:",
            "    m_NormalTrigger: Normal",
            "    m_HighlightedTrigger: Highlighted",
            "    m_PressedTrigger: Pressed",
            "    m_SelectedTrigger: Selected",
            "    m_DisabledTrigger: Disabled",
            "  m_Interactable: 1",
            f"  m_TargetGraphic: {{fileID: {n.img_id}}}",
            "  m_OnClick:",
            "    m_PersistentCalls:",
            "      m_Calls: []",
        ]

    if n.sh_id is not None:
        lines += [
            f"--- !u!114 &{n.sh_id}",
            "MonoBehaviour:",
            "  m_ObjectHideFlags: 0",
            "  m_CorrespondingSourceObject: {fileID: 0}",
            "  m_PrefabInstance: {fileID: 0}",
            "  m_PrefabAsset: {fileID: 0}",
            f"  m_GameObject: {{fileID: {n.go}}}",
            "  m_Enabled: 1",
            "  m_EditorHideFlags: 0",
            f"  m_Script: {{fileID: 11500000, guid: {GUID_SHADOW}, type: 3}}",
            "  m_Name: ",
            "  m_EditorClassIdentifier: ",
            "  m_EffectColor: {r: 0, g: 0, b: 0, a: 0.42}",
            "  m_EffectDistance: {x: 0, y: -4}",
            "  m_UseGraphicAlpha: 1",
        ]

    for ch in n.children:
        emit_node(lines, ch, n.rt)


def txt(name, value, font_size, color, align, **kw) -> Node:
    return Node(name=name, text=value, font_size=font_size, text_color=color, align=align, **kw)


def img(name, color, **kw) -> Node:
    return Node(name=name, image=color, **kw)


def btn(name, label, bg, fg, label_size=26, **kw) -> Node:
    label_n = txt("Label", label, label_size, fg, 4, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(0, 0))
    return Node(name=name, image=bg, button=True, raycast=1, children=[label_n], **kw)


def find_node(root: Node, name: str) -> Node | None:
    if root.name == name:
        return root
    for ch in root.children:
        found = find_node(ch, name)
        if found is not None:
            return found
    return None


def disable_component(text: str, file_id: int) -> str:
    # Flip m_Enabled: 1 → 0 on the MonoBehaviour with this fileID.
    pattern = rf"(--- !u!114 &{file_id}\nMonoBehaviour:\n(?:.*\n)*?  m_Enabled: )1"
    return re.sub(pattern, r"\g<1>0", text, count=1)


def strip_existing(text: str, marker: str) -> str:
    if marker not in text:
        return text
    # Remove from marker comment to next top-level sprint marker or EOF.
    return re.sub(
        rf"\n# --- {re.escape(marker)} ---.*?(?=\n# --- SPRINT-23\.|\Z)",
        "\n",
        text,
        flags=re.S,
    )


def build_hud() -> tuple[list[str], int, int]:
    """Returns YAML lines, canvas_rt, eventsystem_tr."""
    lines: list[str] = []
    canvas_go, canvas_rt = nid(), nid()
    canvas_comp, scaler_id, ray_id, ctrl_id = nid(), nid(), nid(), nid()

    safe = Node(
        name="SafeArea",
        amin=(0, 0),
        amax=(1, 1),
        pos=(0, -9),
        size=(-96, -86),
        pivot=(0.5, 0.5),
    )

    # Top-left position + lap
    pos_panel = Node(
        name="PositionPanel",
        amin=(0, 1),
        amax=(0, 1),
        pivot=(0, 1),
        pos=(24, -16),
        size=(220, 110),
        image=PANEL_BORDER,
        shadow=True,
        children=[
            img("Fill", PANEL_BG, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(-6, -6)),
            txt("PositionText", "1st", 48, GOLD_BRIGHT, 3, amin=(0, 0.35), amax=(1, 1), pos=(0, 0), size=(-20, -8)),
            txt("LapText", "LAP 1/3", 20, MUTED, 3, amin=(0, 0), amax=(1, 0.42), pos=(0, 0), size=(-20, -8)),
        ],
    )

    dist_panel = Node(
        name="DistancePanel",
        amin=(0.5, 1),
        amax=(0.5, 1),
        pivot=(0.5, 1),
        pos=(0, -16),
        size=(240, 64),
        image=PANEL_BORDER,
        shadow=True,
        children=[
            img("Fill", PANEL_BG, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(-6, -6)),
            txt("DistanceText", "125 m", 32, WHITE, 4, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(-16, -8)),
        ],
    )

    currency = Node(
        name="CurrencyPanel",
        amin=(1, 1),
        amax=(1, 1),
        pivot=(1, 1),
        pos=(-120, -16),
        size=(260, 100),
        image=PANEL_BORDER,
        shadow=True,
        children=[
            img("Fill", PANEL_BG, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(-6, -6)),
            txt("CoinsText", "COINS  42", 24, GOLD, 3, amin=(0, 0.45), amax=(1, 1), pos=(0, 0), size=(-24, -8)),
            txt("GemsText", "GEMS  3", 24, GEM, 3, amin=(0, 0), amax=(1, 0.55), pos=(0, 0), size=(-24, -8)),
        ],
    )

    pause_btn = btn(
        "PauseButton",
        "II",
        BUTTON_DARK,
        GOLD_BRIGHT,
        label_size=28,
        amin=(1, 1),
        amax=(1, 1),
        pivot=(1, 1),
        pos=(-24, -20),
        size=(72, 72),
        shadow=True,
    )

    boost = Node(
        name="BoostMeter",
        amin=(0.5, 0),
        amax=(0.5, 0),
        pivot=(0.5, 0),
        pos=(0, 48),
        size=(520, 36),
        image=PANEL_BORDER,
        shadow=True,
        children=[
            img("Track", PANEL_BG, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(-6, -6)),
            Node(
                name="Fill",
                amin=(0, 0),
                amax=(1, 1),
                pos=(0, 0),
                size=(-10, -10),
                image=BOOST_FILL,
                filled=True,
                fill_amount=0.62,
            ),
            txt("BoostLabel", "BOOST", 16, WHITE, 4, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(0, 0)),
        ],
    )

    notif = Node(
        name="NotificationRoot",
        amin=(0.5, 0.55),
        amax=(0.5, 0.55),
        pivot=(0.5, 0.5),
        pos=(0, 0),
        size=(420, 64),
        active=0,
        image=PANEL_BORDER,
        shadow=True,
        children=[
            img("Fill", PANEL_BG, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(-6, -6)),
            txt("NotificationText", "+10 Coins", 28, GOLD_BRIGHT, 4, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(-20, -8)),
        ],
    )

    resume_btn = btn(
        "ResumeButton",
        "Resume",
        GOLD,
        GOLD_LABEL,
        label_size=30,
        amin=(0.5, 0.5),
        amax=(0.5, 0.5),
        pivot=(0.5, 0.5),
        pos=(0, -20),
        size=(280, 72),
    )
    pause_menu = Node(
        name="PauseMenu",
        amin=(0, 0),
        amax=(1, 1),
        pos=(0, 0),
        size=(0, 0),
        pivot=(0.5, 0.5),
        active=0,
        image=DIM,
        raycast=1,
        children=[
            Node(
                name="PausePanel",
                amin=(0.5, 0.5),
                amax=(0.5, 0.5),
                pivot=(0.5, 0.5),
                pos=(0, 0),
                size=(520, 320),
                image=PANEL_BORDER,
                shadow=True,
                children=[
                    img("Fill", PANEL_BG, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(-6, -6)),
                    txt("Title", "PAUSED", 44, GOLD_BRIGHT, 4, amin=(0, 0.55), amax=(1, 1), pos=(0, 0), size=(-24, -16)),
                    txt("Hint", "Visual placeholder — no time freeze", 18, MUTED, 4, amin=(0, 0.38), amax=(1, 0.58), pos=(0, 0), size=(-40, 0)),
                    resume_btn,
                ],
            )
        ],
    )

    safe.children = [pos_panel, dist_panel, currency, pause_btn, boost, notif, pause_menu]
    safe.prep()

    # Canvas root components
    lines.append("# --- SPRINT-23.3-GAMEPLAY-HUD ---")
    lines.append(f"--- !u!1 &{canvas_go}")
    lines.append("GameObject:")
    lines.append("  m_ObjectHideFlags: 0")
    lines.append("  m_CorrespondingSourceObject: {fileID: 0}")
    lines.append("  m_PrefabInstance: {fileID: 0}")
    lines.append("  m_PrefabAsset: {fileID: 0}")
    lines.append("  serializedVersion: 6")
    lines.append("  m_Component:")
    for c in (canvas_rt, canvas_comp, scaler_id, ray_id, ctrl_id):
        lines.append(f"  - component: {{fileID: {c}}}")
    lines.append("  m_Layer: 5")
    lines.append("  m_Name: GameplayHudCanvas")
    lines.append("  m_TagString: Untagged")
    lines.append("  m_Icon: {fileID: 0}")
    lines.append("  m_NavMeshLayer: 0")
    lines.append("  m_StaticEditorFlags: 0")
    lines.append("  m_IsActive: 1")
    lines += [
        f"--- !u!224 &{canvas_rt}",
        "RectTransform:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {canvas_go}}}",
        "  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}",
        "  m_LocalPosition: {x: 0, y: 0, z: 0}",
        "  m_LocalScale: {x: 0, y: 0, z: 0}",
        "  m_ConstrainProportionsScale: 0",
        "  m_Children:",
        f"  - {{fileID: {safe.rt}}}",
        "  m_Father: {fileID: 0}",
        "  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}",
        "  m_AnchorMin: {x: 0, y: 0}",
        "  m_AnchorMax: {x: 0, y: 0}",
        "  m_AnchoredPosition: {x: 0, y: 0}",
        "  m_SizeDelta: {x: 0, y: 0}",
        "  m_Pivot: {x: 0, y: 0}",
        f"--- !u!223 &{canvas_comp}",
        "Canvas:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {canvas_go}}}",
        "  m_Enabled: 1",
        "  serializedVersion: 3",
        "  m_RenderMode: 0",
        "  m_Camera: {fileID: 0}",
        "  m_PlaneDistance: 100",
        "  m_PixelPerfect: 0",
        "  m_ReceivesEvents: 1",
        "  m_OverrideSorting: 0",
        "  m_OverridePixelPerfect: 0",
        "  m_SortingBucketNormalizedSize: 0",
        "  m_VertexColorAlwaysGammaSpace: 0",
        "  m_AdditionalShaderChannelsFlag: 0",
        "  m_UpdateRectTransformForStandalone: 0",
        "  m_SortingLayerID: 0",
        "  m_SortingOrder: 20",
        "  m_TargetDisplay: 0",
        f"--- !u!114 &{scaler_id}",
        "MonoBehaviour:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {canvas_go}}}",
        "  m_Enabled: 1",
        "  m_EditorHideFlags: 0",
        f"  m_Script: {{fileID: 11500000, guid: {GUID_SCALER}, type: 3}}",
        "  m_Name: ",
        "  m_EditorClassIdentifier: ",
        "  m_UiScaleMode: 1",
        "  m_ReferencePixelsPerUnit: 100",
        "  m_ScaleFactor: 1",
        "  m_ReferenceResolution: {x: 1920, y: 1080}",
        "  m_ScreenMatchMode: 0",
        "  m_MatchWidthOrHeight: 0.5",
        "  m_PhysicalUnit: 3",
        "  m_FallbackScreenDPI: 96",
        "  m_DefaultSpriteDPI: 96",
        "  m_DynamicPixelsPerUnit: 1",
        "  m_PresetInfoIsWorld: 0",
        f"--- !u!114 &{ray_id}",
        "MonoBehaviour:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {canvas_go}}}",
        "  m_Enabled: 1",
        "  m_EditorHideFlags: 0",
        f"  m_Script: {{fileID: 11500000, guid: {GUID_RAYCASTER}, type: 3}}",
        "  m_Name: ",
        "  m_EditorClassIdentifier: ",
        "  m_IgnoreReversedGraphics: 1",
        "  m_BlockingObjects: 0",
        "  m_BlockingMask:",
        "    serializedVersion: 2",
        "    m_Bits: 4294967295",
    ]

    notif_n = find_node(safe, "NotificationRoot")
    notif_txt = find_node(safe, "NotificationText")
    lines += [
        f"--- !u!114 &{ctrl_id}",
        "MonoBehaviour:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {canvas_go}}}",
        "  m_Enabled: 1",
        "  m_EditorHideFlags: 0",
        f"  m_Script: {{fileID: 11500000, guid: {GUID_HUD}, type: 3}}",
        "  m_Name: ",
        "  m_EditorClassIdentifier: ",
        f"  pauseButton: {{fileID: {pause_btn.btn_id}}}",
        f"  resumeButton: {{fileID: {resume_btn.btn_id}}}",
        f"  pauseMenuPanel: {{fileID: {pause_menu.go}}}",
        f"  notificationRoot: {{fileID: {notif_n.rt}}}",
        f"  notificationText: {{fileID: {notif_txt.txt_id}}}",
        f"  notificationBackground: {{fileID: {notif_n.img_id}}}",
        "  playNotificationDemo: 1",
        "  notificationDemoIntervalSeconds: 4.5",
        "  notificationVisibleSeconds: 1.6",
    ]

    emit_node(lines, safe, canvas_rt)

    # EventSystem
    es_go, es_t, es_es, es_in = nid(), nid(), nid(), nid()
    lines += [
        f"--- !u!1 &{es_go}",
        "GameObject:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        "  serializedVersion: 6",
        "  m_Component:",
        f"  - component: {{fileID: {es_t}}}",
        f"  - component: {{fileID: {es_es}}}",
        f"  - component: {{fileID: {es_in}}}",
        "  m_Layer: 0",
        "  m_Name: EventSystem",
        "  m_TagString: Untagged",
        "  m_Icon: {fileID: 0}",
        "  m_NavMeshLayer: 0",
        "  m_StaticEditorFlags: 0",
        "  m_IsActive: 1",
        f"--- !u!4 &{es_t}",
        "Transform:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {es_go}}}",
        "  serializedVersion: 2",
        "  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}",
        "  m_LocalPosition: {x: 0, y: 0, z: 0}",
        "  m_LocalScale: {x: 1, y: 1, z: 1}",
        "  m_ConstrainProportionsScale: 0",
        "  m_Children: []",
        "  m_Father: {fileID: 0}",
        "  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}",
        f"--- !u!114 &{es_es}",
        "MonoBehaviour:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {es_go}}}",
        "  m_Enabled: 1",
        "  m_EditorHideFlags: 0",
        f"  m_Script: {{fileID: 11500000, guid: {GUID_EVENTSYSTEM}, type: 3}}",
        "  m_Name: ",
        "  m_EditorClassIdentifier: ",
        "  m_FirstSelected: {fileID: 0}",
        "  m_sendNavigationEvents: 1",
        "  m_DragThreshold: 10",
        f"--- !u!114 &{es_in}",
        "MonoBehaviour:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {es_go}}}",
        "  m_Enabled: 1",
        "  m_EditorHideFlags: 0",
        f"  m_Script: {{fileID: 11500000, guid: {GUID_STANDALONE}, type: 3}}",
        "  m_Name: ",
        "  m_EditorClassIdentifier: ",
        "  m_SendPointerHoverToParent: 1",
        "  m_HorizontalAxis: Horizontal",
        "  m_VerticalAxis: Vertical",
        "  m_SubmitButton: Submit",
        "  m_CancelButton: Cancel",
        "  m_InputActionsPerSecond: 10",
        "  m_RepeatDelay: 0.5",
        "  m_ForceModuleActive: 0",
    ]

    return lines, canvas_rt, es_t


def build_player() -> tuple[list[str], int]:
    lines: list[str] = ["# --- SPRINT-23.4-PLAYER-CONTROLLER ---"]
    go, tr, mf, mr, col = nid(), nid(), nid(), nid(), nid()
    swipe, runner, anim = nid(), nid(), nid()
    vis_go, vis_tr, vis_mf, vis_mr = nid(), nid(), nid(), nid()

    lines += [
        f"--- !u!1 &{go}",
        "GameObject:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        "  serializedVersion: 6",
        "  m_Component:",
        f"  - component: {{fileID: {tr}}}",
        f"  - component: {{fileID: {col}}}",
        f"  - component: {{fileID: {swipe}}}",
        f"  - component: {{fileID: {runner}}}",
        f"  - component: {{fileID: {anim}}}",
        "  m_Layer: 0",
        "  m_Name: RunnerPlayer",
        "  m_TagString: Player",
        "  m_Icon: {fileID: 0}",
        "  m_NavMeshLayer: 0",
        "  m_StaticEditorFlags: 0",
        "  m_IsActive: 1",
        f"--- !u!4 &{tr}",
        "Transform:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {go}}}",
        "  serializedVersion: 2",
        "  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}",
        "  m_LocalPosition: {x: 0, y: 0, z: 0}",
        "  m_LocalScale: {x: 1, y: 1, z: 1}",
        "  m_ConstrainProportionsScale: 0",
        "  m_Children:",
        f"  - {{fileID: {vis_tr}}}",
        "  m_Father: {fileID: 0}",
        "  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}",
        f"--- !u!136 &{col}",
        "CapsuleCollider:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {go}}}",
        "  m_Material: {fileID: 0}",
        "  m_IncludeLayers:",
        "    serializedVersion: 2",
        "    m_Bits: 0",
        "  m_ExcludeLayers:",
        "    serializedVersion: 2",
        "    m_Bits: 0",
        "  m_LayerOverridePriority: 0",
        "  m_IsTrigger: 0",
        "  m_ProvidesContacts: 0",
        "  m_Enabled: 1",
        "  serializedVersion: 2",
        "  m_Radius: 0.35",
        "  m_Height: 1.8",
        "  m_Direction: 1",
        "  m_Center: {x: 0, y: 0.9, z: 0}",
        f"--- !u!114 &{swipe}",
        "MonoBehaviour:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {go}}}",
        "  m_Enabled: 1",
        "  m_EditorHideFlags: 0",
        f"  m_Script: {{fileID: 11500000, guid: {GUID_SWIPE}, type: 3}}",
        "  m_Name: ",
        "  m_EditorClassIdentifier: ",
        "  swipeThresholdPixels: 48",
        f"--- !u!114 &{runner}",
        "MonoBehaviour:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {go}}}",
        "  m_Enabled: 1",
        "  m_EditorHideFlags: 0",
        f"  m_Script: {{fileID: 11500000, guid: {GUID_RUNNER}, type: 3}}",
        "  m_Name: ",
        "  m_EditorClassIdentifier: ",
        f"  config: {{fileID: 11400000, guid: {GUID_CONFIG}, type: 2}}",
        f"  bodyCollider: {{fileID: {col}}}",
        "  startingLane: 1",
        f"--- !u!114 &{anim}",
        "MonoBehaviour:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {go}}}",
        "  m_Enabled: 1",
        "  m_EditorHideFlags: 0",
        f"  m_Script: {{fileID: 11500000, guid: {GUID_ANIM}, type: 3}}",
        "  m_Name: ",
        "  m_EditorClassIdentifier: ",
        f"  runner: {{fileID: {runner}}}",
        "  animator: {fileID: 0}",
        f"--- !u!1 &{vis_go}",
        "GameObject:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        "  serializedVersion: 6",
        "  m_Component:",
        f"  - component: {{fileID: {vis_tr}}}",
        f"  - component: {{fileID: {vis_mf}}}",
        f"  - component: {{fileID: {vis_mr}}}",
        "  m_Layer: 0",
        "  m_Name: Visual",
        "  m_TagString: Untagged",
        "  m_Icon: {fileID: 0}",
        "  m_NavMeshLayer: 0",
        "  m_StaticEditorFlags: 0",
        "  m_IsActive: 1",
        f"--- !u!4 &{vis_tr}",
        "Transform:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {vis_go}}}",
        "  serializedVersion: 2",
        "  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}",
        "  m_LocalPosition: {x: 0, y: 0.9, z: 0}",
        "  m_LocalScale: {x: 0.7, y: 1.8, z: 0.7}",
        "  m_ConstrainProportionsScale: 0",
        "  m_Children: []",
        f"  m_Father: {{fileID: {tr}}}",
        "  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}",
        f"--- !u!33 &{vis_mf}",
        "MeshFilter:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {vis_go}}}",
        f"  m_Mesh: {CAPSULE_MESH}",
        f"--- !u!23 &{vis_mr}",
        "MeshRenderer:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {vis_go}}}",
        "  m_Enabled: 1",
        "  m_CastShadows: 1",
        "  m_ReceiveShadows: 1",
        "  m_DynamicOccludee: 1",
        "  m_StaticShadowCaster: 0",
        "  m_MotionVectors: 1",
        "  m_LightProbeUsage: 1",
        "  m_ReflectionProbeUsage: 1",
        "  m_RayTracingMode: 2",
        "  m_RayTraceProcedural: 0",
        "  m_RenderingLayerMask: 1",
        "  m_RendererPriority: 0",
        "  m_Materials:",
        f"  - {LIT_MAT}",
        "  m_StaticBatchInfo:",
        "    firstSubMesh: 0",
        "    subMeshCount: 0",
        "  m_StaticBatchRoot: {fileID: 0}",
        "  m_ProbeAnchor: {fileID: 0}",
        "  m_LightProbeVolumeOverride: {fileID: 0}",
        "  m_ScaleInLightmap: 1",
        "  m_ReceiveGI: 1",
        "  m_PreserveUVs: 0",
        "  m_IgnoreNormalsForChartDetection: 0",
        "  m_ImportantGI: 0",
        "  m_StitchLightmapSeams: 1",
        "  m_SelectedEditorRenderState: 3",
        "  m_MinimumChartSize: 4",
        "  m_AutoUVMaxDistance: 0.5",
        "  m_AutoUVMaxAngle: 89",
        "  m_LightmapParameters: {fileID: 0}",
        "  m_SortingLayerID: 0",
        "  m_SortingLayer: 0",
        "  m_SortingOrder: 0",
        "  m_AdditionalVertexStreams: {fileID: 0}",
    ]

    # Ground strip
    g_go, g_tr, g_mf, g_mr, g_box = nid(), nid(), nid(), nid(), nid()
    lines += [
        f"--- !u!1 &{g_go}",
        "GameObject:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        "  serializedVersion: 6",
        "  m_Component:",
        f"  - component: {{fileID: {g_tr}}}",
        f"  - component: {{fileID: {g_mf}}}",
        f"  - component: {{fileID: {g_mr}}}",
        f"  - component: {{fileID: {g_box}}}",
        "  m_Layer: 0",
        "  m_Name: RunnerGround",
        "  m_TagString: Untagged",
        "  m_Icon: {fileID: 0}",
        "  m_NavMeshLayer: 0",
        "  m_StaticEditorFlags: 0",
        "  m_IsActive: 1",
        f"--- !u!4 &{g_tr}",
        "Transform:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {g_go}}}",
        "  serializedVersion: 2",
        "  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}",
        "  m_LocalPosition: {x: 0, y: -0.05, z: 40}",
        "  m_LocalScale: {x: 12, y: 0.1, z: 120}",
        "  m_ConstrainProportionsScale: 0",
        "  m_Children: []",
        "  m_Father: {fileID: 0}",
        "  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}",
        f"--- !u!33 &{g_mf}",
        "MeshFilter:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {g_go}}}",
        f"  m_Mesh: {CUBE_MESH}",
        f"--- !u!23 &{g_mr}",
        "MeshRenderer:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {g_go}}}",
        "  m_Enabled: 1",
        "  m_CastShadows: 1",
        "  m_ReceiveShadows: 1",
        "  m_DynamicOccludee: 1",
        "  m_StaticShadowCaster: 0",
        "  m_MotionVectors: 1",
        "  m_LightProbeUsage: 1",
        "  m_ReflectionProbeUsage: 1",
        "  m_RayTracingMode: 2",
        "  m_RayTraceProcedural: 0",
        "  m_RenderingLayerMask: 1",
        "  m_RendererPriority: 0",
        "  m_Materials:",
        f"  - {LIT_MAT}",
        "  m_StaticBatchInfo:",
        "    firstSubMesh: 0",
        "    subMeshCount: 0",
        "  m_StaticBatchRoot: {fileID: 0}",
        "  m_ProbeAnchor: {fileID: 0}",
        "  m_LightProbeVolumeOverride: {fileID: 0}",
        "  m_ScaleInLightmap: 1",
        "  m_ReceiveGI: 1",
        "  m_PreserveUVs: 0",
        "  m_IgnoreNormalsForChartDetection: 0",
        "  m_ImportantGI: 0",
        "  m_StitchLightmapSeams: 1",
        "  m_SelectedEditorRenderState: 3",
        "  m_MinimumChartSize: 4",
        "  m_AutoUVMaxDistance: 0.5",
        "  m_AutoUVMaxAngle: 89",
        "  m_LightmapParameters: {fileID: 0}",
        "  m_SortingLayerID: 0",
        "  m_SortingLayer: 0",
        "  m_SortingOrder: 0",
        "  m_AdditionalVertexStreams: {fileID: 0}",
        f"--- !u!65 &{g_box}",
        "BoxCollider:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {g_go}}}",
        "  m_Material: {fileID: 0}",
        "  m_IncludeLayers:",
        "    serializedVersion: 2",
        "    m_Bits: 0",
        "  m_ExcludeLayers:",
        "    serializedVersion: 2",
        "    m_Bits: 0",
        "  m_LayerOverridePriority: 0",
        "  m_IsTrigger: 0",
        "  m_ProvidesContacts: 0",
        "  m_Enabled: 1",
        "  serializedVersion: 3",
        "  m_Size: {x: 1, y: 1, z: 1}",
        "  m_Center: {x: 0, y: 0, z: 0}",
    ]
    return lines, tr


def patch_camera(text: str, player_tr: int | None) -> str:
    """Disable SideScrollCameraFollow; add RunnerCameraFollow + shake + fx stubs."""
    if "SPRINT-23.5-CAMERA-FOLLOW" in text and "RunnerCameraFollow" in text:
        return text

    # Disable existing SideScrollCameraFollow (705507997) and PodiumCameraDirector stays.
    text = disable_component(text, 705507997)

    follow_id, shake_id, fx_id = nid(), nid(), nid()
    target_ref = f"{{fileID: {player_tr}}}" if player_tr else "{fileID: 0}"

    # Inject component refs onto Main Camera GameObject component list.
    old_cam_comps = """  m_Component:
  - component: {fileID: 705507996}
  - component: {fileID: 705507995}
  - component: {fileID: 705507994}
  - component: {fileID: 705507997}
  - component: {fileID: 705507998}"""
    new_cam_comps = f"""  m_Component:
  - component: {{fileID: 705507996}}
  - component: {{fileID: 705507995}}
  - component: {{fileID: 705507994}}
  - component: {{fileID: 705507997}}
  - component: {{fileID: 705507998}}
  - component: {{fileID: {follow_id}}}
  - component: {{fileID: {shake_id}}}
  - component: {{fileID: {fx_id}}}"""
    if old_cam_comps not in text:
        raise SystemExit("Main Camera component list not found for camera patch.")
    text = text.replace(old_cam_comps, new_cam_comps, 1)

    cam_yaml = f"""
# --- SPRINT-23.5-CAMERA-FOLLOW ---
--- !u!114 &{follow_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 705507993}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_CAM_FOLLOW}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  target: {target_ref}
  followSpeed: 10
  rotationSpeed: 8
  cameraOffset: {{x: 0, y: 4.5, z: -8.5}}
  verticalOffset: 0
  horizontalOffset: 0
  smoothTime: 0.12
  verticalSmoothTime: 0.28
  groundedVerticalSmoothTime: 0.4
  groundedYThreshold: 0.35
  lookAtHeight: 1.2
  fieldOfView: 60
--- !u!114 &{shake_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 705507993}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_CAM_SHAKE}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  defaultIntensity: 0.25
  defaultDuration: 0.2
--- !u!114 &{fx_id}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 705507993}}
  m_Enabled: 0
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_CAM_FX}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  enableSpeedFovIncrease: 0
  enableMotionBlur: 0
  enableCinematicTransitions: 0
"""
    # Place camera components after existing PodiumCameraDirector block.
    anchor = "--- !u!114 &705507998\n"
    idx = text.find(anchor)
    if idx < 0:
        raise SystemExit("PodiumCameraDirector block not found.")
    # Find end of that MonoBehaviour (next ---)
    end = text.find("\n--- !u!", idx + len(anchor))
    if end < 0:
        end = len(text)
    text = text[:end] + cam_yaml + text[end:]

    # Nudge Main Camera starting pose behind the runner.
    text = text.replace(
        "  m_LocalPosition: {x: 0, y: 1, z: -10}\n  m_LocalScale: {x: 1, y: 1, z: 1}\n  m_ConstrainProportionsScale: 0\n  m_Children: []\n  m_Father: {fileID: 0}\n  m_RootOrder: 0",
        "  m_LocalPosition: {x: 0, y: 4.5, z: -8.5}\n  m_LocalScale: {x: 1, y: 1, z: 1}\n  m_ConstrainProportionsScale: 0\n  m_Children: []\n  m_Father: {fileID: 0}\n  m_RootOrder: 0",
        1,
    )
    return text

def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--hud", action="store_true")
    parser.add_argument("--player", action="store_true")
    parser.add_argument("--camera", action="store_true")
    parser.add_argument("--all", action="store_true")
    args = parser.parse_args()
    do_all = args.all or not (args.hud or args.player or args.camera)
    do_hud = do_all or args.hud
    do_player = do_all or args.player
    do_camera = do_all or args.camera

    text = SCENE.read_text(encoding="utf-8")
    player_tr = None

    if do_hud:
        text = strip_existing(text, "SPRINT-23.3-GAMEPLAY-HUD")
        for fid in (500300011, 500300012, 500300014):
            text = disable_component(text, fid)
        hud_lines, _, _ = build_hud()
        joined = "\n".join(hud_lines)
        if "m_Name: EventSystem" in text:
            joined = re.sub(
                r"\n--- !u!1 &\d+\nGameObject:\n(?:.*\n)*?  m_Name: EventSystem\n(?:.*\n)*?  m_ForceModuleActive: 0\n?",
                "\n",
                joined,
                count=1,
            )
        text = text.rstrip() + "\n" + joined + "\n"
        print("Patched HUD.")

    if do_player:
        text = strip_existing(text, "SPRINT-23.4-PLAYER-CONTROLLER")
        player_lines, player_tr = build_player()
        text = text.rstrip() + "\n" + "\n".join(player_lines) + "\n"
        print(f"Patched player (tr={player_tr}).")

    if do_camera:
        if player_tr is None:
            m4 = re.search(
                r"--- !u!1 &\d+\nGameObject:\n(?:.*\n)*?  m_Name: RunnerPlayer\n(?:.*\n)*?  m_Component:\n  - component: \{fileID: (\d+)\}",
                text,
            )
            if m4:
                player_tr = int(m4.group(1))
        if "SPRINT-23.5-CAMERA-FOLLOW" in text:
            print("Camera already patched; skipping.")
        else:
            text = patch_camera(text, player_tr)
            print(f"Patched camera (target tr={player_tr}).")

    SCENE.write_text(text, encoding="utf-8")
    print(f"Wrote {SCENE}")


if __name__ == "__main__":
    main()
