#!/usr/bin/env python3
"""Generate LobbyScreen.unity (Sprint 21.4 Host Controls UI) without Unity batchmode."""

from __future__ import annotations

from dataclasses import dataclass, field
from pathlib import Path

OUT = Path(r"C:\Projects\GulfRun\Client\Assets\_Project\Scenes\LobbyScreen.unity")

GUID_TEXT = "5f7201a12d95ffc409449d95f23cf332"
GUID_IMAGE = "fe87c0e1cc204ed48ad3b37840f39efc"
GUID_BUTTON = "4e29b1a8efbd4b44bb3f3716e73f07ff"
GUID_SCALER = "0cd44c1031e13a943bb63640046fad76"
GUID_RAYCASTER = "dc42784cf147c0c48a680349fa168899"
GUID_SHADOW = "cfabb0440166ab443bba8876756fdfa9"
GUID_MASK = "31a19414c41e5ae4aae2af33fee712f6"
GUID_EVENTSYSTEM = "76c392e42b5098c458856cdf6ecaaaa1"
GUID_STANDALONE = "4f231c4fb786f3946a6b90b886c48677"
GUID_CONTROLLER = "b20c0000000000000000000000000072"
GUID_BG = "a18b0000000000000000000000000001"
KNOB = "{fileID: 10913, guid: 0000000000000000f000000000000000, type: 0}"
FONT = "{fileID: 10102, guid: 0000000000000000e000000000000000, type: 0}"
BG_SPRITE = f"{{fileID: 21300000, guid: {GUID_BG}, type: 3}}"

GOLD = (0.90, 0.71, 0.25, 1.0)
GOLD_BRIGHT = (1.0, 0.84, 0.40, 1.0)
PANEL_BG = (0.10, 0.09, 0.10, 0.78)
PANEL_BORDER = (0.90, 0.71, 0.25, 0.55)
WHITE = (1.0, 1.0, 1.0, 1.0)
MUTED = (0.80, 0.80, 0.80, 1.0)
BUTTON_DARK = (0.12, 0.10, 0.09, 0.92)
CARD = (0.12, 0.11, 0.12, 0.88)
SUCCESS = (0.40, 0.85, 0.45, 1.0)
READY_MUTED = (0.55, 0.55, 0.55, 1.0)
CONNECTING = (0.95, 0.72, 0.28, 1.0)
ONLINE = (0.35, 0.90, 0.48, 1.0)
EMPTY_FILL = (0.10, 0.09, 0.10, 0.55)
EMPTY_BORDER = (0.90, 0.71, 0.25, 0.22)
AVATAR = (0.72, 0.58, 0.42, 1.0)
GOLD_LABEL = (0.20, 0.14, 0.02, 1.0)

_next = 10000


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
    scale: tuple[float, float, float] = (1.0, 1.0, 1.0)
    # visual
    image: tuple[float, float, float, float] | None = None
    sprite: str = "{fileID: 0}"
    preserve: int = 0
    raycast: int = 0
    shadow: bool = False
    mask: bool = False
    text: str | None = None
    font_size: int = 24
    font_style: int = 1
    align: int = 4
    text_color: tuple[float, float, float, float] = WHITE
    button: bool = False
    interactable: int = 1
    transition: int = 1
    # ids for optional components
    cr: int = field(default_factory=nid)
    img_id: int | None = None
    txt_id: int | None = None
    btn_id: int | None = None
    sh_id: int | None = None
    mask_id: int | None = None

    def prep(self) -> None:
        needs_cr = self.image is not None or self.text is not None or self.button
        if needs_cr or self.shadow or self.mask:
            pass  # cr always allocated
        if self.image is not None or self.button:
            self.img_id = nid()
        if self.text is not None:
            self.txt_id = nid()
        if self.button:
            self.btn_id = nid()
        if self.shadow:
            self.sh_id = nid()
        if self.mask:
            self.mask_id = nid()
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
    if n.mask_id:
        out.append(n.mask_id)
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
    lines.append("  m_Layer: 0")
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
    lines.append(f"  m_LocalScale: {{x: {n.scale[0]}, y: {n.scale[1]}, z: {n.scale[2]}}}")
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
        lines.append(f"--- !u!222 &{n.cr}")
        lines.append("CanvasRenderer:")
        lines.append("  m_ObjectHideFlags: 0")
        lines.append("  m_CorrespondingSourceObject: {fileID: 0}")
        lines.append("  m_PrefabInstance: {fileID: 0}")
        lines.append("  m_PrefabAsset: {fileID: 0}")
        lines.append(f"  m_GameObject: {{fileID: {n.go}}}")
        lines.append("  m_CullTransparentMesh: 1")

    if n.img_id is not None:
        color = n.image if n.image is not None else (1, 1, 1, 1)
        lines.append(f"--- !u!114 &{n.img_id}")
        lines.append("MonoBehaviour:")
        lines.append("  m_ObjectHideFlags: 0")
        lines.append("  m_CorrespondingSourceObject: {fileID: 0}")
        lines.append("  m_PrefabInstance: {fileID: 0}")
        lines.append("  m_PrefabAsset: {fileID: 0}")
        lines.append(f"  m_GameObject: {{fileID: {n.go}}}")
        lines.append("  m_Enabled: 1")
        lines.append("  m_EditorHideFlags: 0")
        lines.append(f"  m_Script: {{fileID: 11500000, guid: {GUID_IMAGE}, type: 3}}")
        lines.append("  m_Name: ")
        lines.append("  m_EditorClassIdentifier: ")
        lines.append("  m_Material: {fileID: 0}")
        lines.append(f"  m_Color: {c4(color)}")
        lines.append(f"  m_RaycastTarget: {n.raycast}")
        lines.append("  m_RaycastPadding: {x: 0, y: 0, z: 0, w: 0}")
        lines.append("  m_Maskable: 1")
        lines.append("  m_OnCullStateChanged:")
        lines.append("    m_PersistentCalls:")
        lines.append("      m_Calls: []")
        lines.append(f"  m_Sprite: {n.sprite}")
        lines.append("  m_Type: 0")
        lines.append(f"  m_PreserveAspect: {n.preserve}")
        lines.append("  m_FillCenter: 1")
        lines.append("  m_FillMethod: 4")
        lines.append("  m_FillAmount: 1")
        lines.append("  m_FillClockwise: 1")
        lines.append("  m_FillOrigin: 0")
        lines.append("  m_UseSpriteMesh: 0")
        lines.append("  m_PixelsPerUnitMultiplier: 1")

    if n.txt_id is not None:
        lines.append(f"--- !u!114 &{n.txt_id}")
        lines.append("MonoBehaviour:")
        lines.append("  m_ObjectHideFlags: 0")
        lines.append("  m_CorrespondingSourceObject: {fileID: 0}")
        lines.append("  m_PrefabInstance: {fileID: 0}")
        lines.append("  m_PrefabAsset: {fileID: 0}")
        lines.append(f"  m_GameObject: {{fileID: {n.go}}}")
        lines.append("  m_Enabled: 1")
        lines.append("  m_EditorHideFlags: 0")
        lines.append(f"  m_Script: {{fileID: 11500000, guid: {GUID_TEXT}, type: 3}}")
        lines.append("  m_Name: ")
        lines.append("  m_EditorClassIdentifier: ")
        lines.append("  m_Material: {fileID: 0}")
        lines.append(f"  m_Color: {c4(n.text_color)}")
        lines.append("  m_RaycastTarget: 0")
        lines.append("  m_RaycastPadding: {x: 0, y: 0, z: 0, w: 0}")
        lines.append("  m_Maskable: 1")
        lines.append("  m_OnCullStateChanged:")
        lines.append("    m_PersistentCalls:")
        lines.append("      m_Calls: []")
        lines.append("  m_FontData:")
        lines.append(f"    m_Font: {FONT}")
        lines.append(f"    m_FontSize: {n.font_size}")
        lines.append(f"    m_FontStyle: {n.font_style}")
        lines.append("    m_BestFit: 0")
        lines.append("    m_MinSize: 10")
        lines.append("    m_MaxSize: 40")
        lines.append(f"    m_Alignment: {n.align}")
        lines.append("    m_AlignByGeometry: 0")
        lines.append("    m_RichText: 1")
        lines.append("    m_HorizontalOverflow: 0")
        lines.append("    m_VerticalOverflow: 1")
        lines.append("    m_LineSpacing: 1")
        lines.append(f"  m_Text: {n.text}")

    if n.btn_id is not None:
        lines.append(f"--- !u!114 &{n.btn_id}")
        lines.append("MonoBehaviour:")
        lines.append("  m_ObjectHideFlags: 0")
        lines.append("  m_CorrespondingSourceObject: {fileID: 0}")
        lines.append("  m_PrefabInstance: {fileID: 0}")
        lines.append("  m_PrefabAsset: {fileID: 0}")
        lines.append(f"  m_GameObject: {{fileID: {n.go}}}")
        lines.append("  m_Enabled: 1")
        lines.append("  m_EditorHideFlags: 0")
        lines.append(f"  m_Script: {{fileID: 11500000, guid: {GUID_BUTTON}, type: 3}}")
        lines.append("  m_Name: ")
        lines.append("  m_EditorClassIdentifier: ")
        lines.append("  m_Navigation:")
        lines.append("    m_Mode: 3")
        lines.append("    m_WrapAround: 0")
        lines.append("    m_SelectOnUp: {fileID: 0}")
        lines.append("    m_SelectOnDown: {fileID: 0}")
        lines.append("    m_SelectOnLeft: {fileID: 0}")
        lines.append("    m_SelectOnRight: {fileID: 0}")
        lines.append(f"  m_Transition: {n.transition}")
        lines.append("  m_Colors:")
        lines.append("    m_NormalColor: {r: 1, g: 1, b: 1, a: 1}")
        lines.append("    m_HighlightedColor: {r: 0.96, g: 0.96, b: 0.96, a: 1}")
        lines.append("    m_PressedColor: {r: 0.78, g: 0.78, b: 0.78, a: 1}")
        lines.append("    m_SelectedColor: {r: 0.96, g: 0.96, b: 0.96, a: 1}")
        lines.append("    m_DisabledColor: {r: 0.18, g: 0.16, b: 0.14, a: 0.55}")
        lines.append("    m_ColorMultiplier: 1")
        lines.append("    m_FadeDuration: 0.1")
        lines.append("  m_SpriteState:")
        lines.append("    m_HighlightedSprite: {fileID: 0}")
        lines.append("    m_PressedSprite: {fileID: 0}")
        lines.append("    m_SelectedSprite: {fileID: 0}")
        lines.append("    m_DisabledSprite: {fileID: 0}")
        lines.append("  m_AnimationTriggers:")
        lines.append("    m_NormalTrigger: Normal")
        lines.append("    m_HighlightedTrigger: Highlighted")
        lines.append("    m_PressedTrigger: Pressed")
        lines.append("    m_SelectedTrigger: Selected")
        lines.append("    m_DisabledTrigger: Disabled")
        lines.append(f"  m_Interactable: {n.interactable}")
        lines.append(f"  m_TargetGraphic: {{fileID: {n.img_id}}}")
        lines.append("  m_OnClick:")
        lines.append("    m_PersistentCalls:")
        lines.append("      m_Calls: []")

    if n.sh_id is not None:
        lines.append(f"--- !u!114 &{n.sh_id}")
        lines.append("MonoBehaviour:")
        lines.append("  m_ObjectHideFlags: 0")
        lines.append("  m_CorrespondingSourceObject: {fileID: 0}")
        lines.append("  m_PrefabInstance: {fileID: 0}")
        lines.append("  m_PrefabAsset: {fileID: 0}")
        lines.append(f"  m_GameObject: {{fileID: {n.go}}}")
        lines.append("  m_Enabled: 1")
        lines.append("  m_EditorHideFlags: 0")
        lines.append(f"  m_Script: {{fileID: 11500000, guid: {GUID_SHADOW}, type: 3}}")
        lines.append("  m_Name: ")
        lines.append("  m_EditorClassIdentifier: ")
        lines.append("  m_EffectColor: {r: 0, g: 0, b: 0, a: 0.55}")
        lines.append("  m_EffectDistance: {x: 0, y: -8}")
        lines.append("  m_UseGraphicAlpha: 1")

    if n.mask_id is not None:
        lines.append(f"--- !u!114 &{n.mask_id}")
        lines.append("MonoBehaviour:")
        lines.append("  m_ObjectHideFlags: 0")
        lines.append("  m_CorrespondingSourceObject: {fileID: 0}")
        lines.append("  m_PrefabInstance: {fileID: 0}")
        lines.append("  m_PrefabAsset: {fileID: 0}")
        lines.append(f"  m_GameObject: {{fileID: {n.go}}}")
        lines.append("  m_Enabled: 1")
        lines.append("  m_EditorHideFlags: 0")
        lines.append(f"  m_Script: {{fileID: 11500000, guid: {GUID_MASK}, type: 3}}")
        lines.append("  m_Name: ")
        lines.append("  m_EditorClassIdentifier: ")
        lines.append("  m_ShowMaskGraphic: 1")

    for ch in n.children:
        emit_node(lines, ch, n.rt)


def txt(name: str, value: str, font_size: int, color, align: int, **kw) -> Node:
    return Node(name=name, text=value, font_size=font_size, text_color=color, align=align, **kw)


def img(name: str, color, **kw) -> Node:
    return Node(name=name, image=color, **kw)


def btn(name: str, label: str, bg, fg, label_size: int = 24, **kw) -> Node:
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


def host_badge(visible: bool) -> Node:
    return img(
        "HostBadge",
        GOLD_BRIGHT,
        active=1 if visible else 0,
        amin=(0, 0.5),
        amax=(0, 0.5),
        pos=(460, 22),
        size=(96, 34),
        pivot=(0, 0.5),
        children=[
            img(
                "HostBadgeInner",
                GOLD,
                amin=(0, 0),
                amax=(1, 1),
                pos=(0, 0),
                size=(-4, -4),
            ),
            txt("HostLabel", "HOST", 16, GOLD_LABEL, 4, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(0, 0)),
        ],
    )


def kick_button(visible: bool) -> Node:
    return btn(
        "KickButton",
        "Kick",
        (0.42, 0.14, 0.12, 0.92),
        (1.0, 0.82, 0.78, 1.0),
        label_size=18,
        active=1 if visible else 0,
        amin=(1, 0.5),
        amax=(1, 0.5),
        pos=(-36, 28),
        size=(88, 40),
        pivot=(1, 0.5),
        interactable=0,
        transition=0,
    )


def player_slot(
    index: int,
    occupied: bool,
    pname: str,
    country: str,
    flag,
    level: int,
    ready_label: str,
    ready_color,
    online: bool,
    show_host: bool,
    show_kick: bool,
) -> Node:
    slot_h, gap = 148.0, 22.0
    total = slot_h * 4 + gap * 3
    y = (total * 0.5 - slot_h * 0.5) - index * (slot_h + gap)
    kids: list[Node] = [
        img("Fill", CARD if occupied else EMPTY_FILL, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(-8, -8)),
        host_badge(show_host),
    ]
    if index != 0:
        kids.append(kick_button(show_kick))
    if occupied:
        online_dot = img(
            "OnlineIndicator",
            ONLINE if online else READY_MUTED,
            amin=(1, 0),
            amax=(1, 0),
            pos=(-6, 8),
            size=(22, 22),
            sprite=KNOB,
            preserve=1,
        )
        avatar_img = img(
            "AvatarImage",
            AVATAR,
            amin=(0, 0),
            amax=(1, 1),
            pos=(0, 0),
            size=(-12, -12),
            sprite=KNOB,
            preserve=1,
        )
        avatar = img(
            "Avatar",
            GOLD_BRIGHT,
            amin=(0, 0.5),
            amax=(0, 0.5),
            pos=(78, 0),
            size=(96, 96),
            sprite=KNOB,
            preserve=1,
            mask=True,
            children=[avatar_img, online_dot],
        )
        kids.extend(
            [
                avatar,
                txt(
                    "PlayerName",
                    pname,
                    30,
                    WHITE,
                    3,
                    amin=(0, 0.5),
                    amax=(0, 0.5),
                    pos=(148, 22),
                    size=(300, 42),
                    pivot=(0, 0.5),
                ),
                img(
                    "CountryFlag",
                    flag,
                    amin=(0, 0.5),
                    amax=(0, 0.5),
                    pos=(148, -24),
                    size=(40, 26),
                    pivot=(0, 0.5),
                ),
                txt(
                    "CountryCode",
                    country,
                    18,
                    MUTED,
                    3,
                    amin=(0, 0.5),
                    amax=(0, 0.5),
                    pos=(196, -24),
                    size=(80, 28),
                    pivot=(0, 0.5),
                ),
                img(
                    "LevelBadge",
                    GOLD,
                    amin=(0.5, 0.5),
                    amax=(0.5, 0.5),
                    pos=(72, 0),
                    size=(78, 78),
                    sprite=KNOB,
                    preserve=1,
                    children=[
                        txt(
                            "LevelText",
                            f"Lv {level}",
                            16,
                            GOLD_LABEL,
                            4,
                            amin=(0, 0),
                            amax=(1, 1),
                            pos=(0, 0),
                            size=(0, 0),
                        )
                    ],
                ),
                img(
                    "ReadyStatus",
                    ready_color,
                    amin=(1, 0.5),
                    amax=(1, 0.5),
                    pos=(-200, 14),
                    size=(24, 24),
                    pivot=(1, 0.5),
                    sprite=KNOB,
                    preserve=1,
                ),
                txt(
                    "ReadyLabel",
                    ready_label,
                    22,
                    ready_color,
                    5,
                    amin=(1, 0.5),
                    amax=(1, 0.5),
                    pos=(-40, -16),
                    size=(180, 34),
                    pivot=(1, 0.5),
                ),
            ]
        )
    else:
        kids.extend(
            [
                img(
                    "EmptyPlusRing",
                    (GOLD_BRIGHT[0], GOLD_BRIGHT[1], GOLD_BRIGHT[2], 0.28),
                    amin=(0.5, 0.5),
                    amax=(0.5, 0.5),
                    pos=(-210, 0),
                    size=(72, 72),
                    sprite=KNOB,
                    preserve=1,
                    children=[
                        txt(
                            "EmptyPlusMark",
                            "+",
                            40,
                            GOLD_BRIGHT,
                            4,
                            amin=(0, 0),
                            amax=(1, 1),
                            pos=(0, 0),
                            size=(0, 0),
                        )
                    ],
                ),
                txt(
                    "EmptySlotLabel",
                    "+ Waiting for Player",
                    28,
                    (MUTED[0], MUTED[1], MUTED[2], 0.92),
                    3,
                    amin=(0.5, 0.5),
                    amax=(0.5, 0.5),
                    pos=(-150, 0),
                    size=(520, 48),
                    pivot=(0, 0.5),
                ),
            ]
        )
    return img(
        f"PlayerSlot_{index}",
        PANEL_BORDER if occupied else EMPTY_BORDER,
        shadow=True,
        amin=(0.5, 0.5),
        amax=(0.5, 0.5),
        pos=(0, y),
        size=(960, 148),
        children=kids,
    )


SCENE_HEADER = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_OcclusionBakeSettings:
    smallestOccluder: 5
    smallestHole: 0.25
    backfaceThreshold: 100
  m_SceneGUID: 00000000000000000000000000000000
  m_OcclusionCullingData: {fileID: 0}
--- !u!104 &2
RenderSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 9
  m_Fog: 0
  m_FogColor: {r: 0.5, g: 0.5, b: 0.5, a: 1}
  m_FogMode: 3
  m_FogDensity: 0.01
  m_LinearFogStart: 0
  m_LinearFogEnd: 300
  m_AmbientSkyColor: {r: 0.212, g: 0.227, b: 0.259, a: 1}
  m_AmbientEquatorColor: {r: 0.114, g: 0.125, b: 0.133, a: 1}
  m_AmbientGroundColor: {r: 0.047, g: 0.043, b: 0.035, a: 1}
  m_AmbientIntensity: 1
  m_AmbientMode: 0
  m_SubtractiveShadowColor: {r: 0.42, g: 0.478, b: 0.627, a: 1}
  m_SkyboxMaterial: {fileID: 10304, guid: 0000000000000000f000000000000000, type: 0}
  m_HaloStrength: 0.5
  m_FlareStrength: 1
  m_FlareFadeSpeed: 3
  m_HaloTexture: {fileID: 0}
  m_SpotCookie: {fileID: 10001, guid: 0000000000000000e000000000000000, type: 0}
  m_DefaultReflectionMode: 0
  m_DefaultReflectionResolution: 128
  m_ReflectionBounces: 1
  m_ReflectionIntensity: 1
  m_CustomReflection: {fileID: 0}
  m_Sun: {fileID: 0}
  m_UseRadianceAmbientProbe: 0
--- !u!157 &3
LightmapSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 12
  m_GIWorkflowMode: 1
  m_GISettings:
    serializedVersion: 2
    m_BounceScale: 1
    m_IndirectOutputScale: 1
    m_AlbedoBoost: 1
    m_EnvironmentLightingMode: 0
    m_EnableBakedLightmaps: 1
    m_EnableRealtimeLightmaps: 0
  m_LightmapEditorSettings:
    serializedVersion: 12
    m_Resolution: 2
    m_BakeResolution: 40
    m_AtlasSize: 1024
    m_AO: 0
    m_AOMaxDistance: 1
    m_CompAOExponent: 1
    m_CompAOExponentDirect: 0
    m_ExtractAmbientOcclusion: 0
    m_Padding: 2
    m_LightmapParameters: {fileID: 0}
    m_LightmapsBakeMode: 1
    m_TextureCompression: 1
    m_FinalGather: 0
    m_FinalGatherFiltering: 1
    m_FinalGatherRayCount: 256
    m_ReflectionCompression: 2
    m_MixedBakeMode: 2
    m_BakeBackend: 1
    m_PVRSampling: 1
    m_PVRDirectSampleCount: 32
    m_PVRSampleCount: 512
    m_PVRBounces: 2
    m_PVREnvironmentSampleCount: 256
    m_PVREnvironmentReferencePointCount: 2048
    m_PVRFilteringMode: 1
    m_PVRDenoiserTypeDirect: 1
    m_PVRDenoiserTypeIndirect: 1
    m_PVRDenoiserTypeAO: 1
    m_PVRFilterTypeDirect: 0
    m_PVRFilterTypeIndirect: 0
    m_PVRFilterTypeAO: 0
    m_PVREnvironmentMIS: 1
    m_PVRCulling: 1
    m_PVRFilteringGaussRadiusDirect: 1
    m_PVRFilteringGaussRadiusIndirect: 5
    m_PVRFilteringGaussRadiusAO: 2
    m_PVRFilteringAtrousPositionSigmaDirect: 0.5
    m_PVRFilteringAtrousPositionSigmaIndirect: 2
    m_PVRFilteringAtrousPositionSigmaAO: 1
    m_ExportTrainingData: 0
    m_TrainingDataDestination: TrainingData
    m_LightProbeSampleCountMultiplier: 4
  m_LightingDataAsset: {fileID: 0}
  m_LightingSettings: {fileID: 0}
--- !u!196 &4
NavMeshSettings:
  serializedVersion: 2
  m_ObjectHideFlags: 0
  m_BuildSettings:
    serializedVersion: 3
    agentTypeID: 0
    agentRadius: 0.5
    agentHeight: 2
    agentSlope: 45
    agentClimb: 0.4
    ledgeDropHeight: 0
    maxJumpAcrossDistance: 0
    minRegionArea: 2
    manualCellSize: 0
    cellSize: 0.16666667
    manualTileSize: 0
    tileSize: 256
    buildHeightMesh: 0
    maxJobWorkers: 0
    preserveTilesOutsideBounds: 0
    debug:
      m_Flags: 0
  m_NavMeshData: {fileID: 0}
"""


def main() -> None:
    global _next
    _next = 10000

    back = btn(
        "BackButton",
        "Back",
        BUTTON_DARK,
        GOLD_BRIGHT,
        amin=(0, 1),
        amax=(0, 1),
        pos=(48, -40),
        size=(168, 64),
        pivot=(0, 1),
    )

    header = img(
        "HeaderRoot",
        PANEL_BORDER,
        shadow=True,
        amin=(0.5, 1),
        amax=(0.5, 1),
        pos=(0, -32),
        size=(1180, 128),
        pivot=(0.5, 1),
        children=[
            img("Fill", PANEL_BG, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(-6, -6)),
            txt(
                "RoomTypeText",
                "Room Type: Public",
                32,
                GOLD_BRIGHT,
                4,
                amin=(0, 0.52),
                amax=(1, 1),
                pos=(0, -3),
                size=(-48, 0),
            ),
            txt(
                "HostNameText",
                "Host: DesertFox",
                22,
                WHITE,
                3,
                amin=(0, 0),
                amax=(0.38, 0.52),
                pos=(14, 3),
                size=(-44, -14),
                pivot=(0, 0.5),
            ),
            txt(
                "PlayerCountText",
                "Players: 1 / 4",
                22,
                WHITE,
                4,
                amin=(0.32, 0),
                amax=(0.68, 0.52),
                pos=(0, 3),
                size=(-16, -14),
            ),
            txt(
                "RoomCodeText",
                "GULF-4821",
                22,
                GOLD,
                5,
                amin=(0.62, 0),
                amax=(1, 0.52),
                pos=(-14, 3),
                size=(-44, -14),
                pivot=(1, 0.5),
            ),
        ],
    )

    slots = Node(
        name="SlotsRoot",
        amin=(0.5, 0.5),
        amax=(0.5, 0.5),
        pos=(0, 78),
        size=(980, 640),
        children=[
            player_slot(0, True, "DesertFox", "KW", (0.05, 0.45, 0.25, 1), 12, "Ready", SUCCESS, True, True, False),
            player_slot(1, True, "NightOwl", "AE", (0.05, 0.28, 0.55, 1), 8, "Not Ready", READY_MUTED, True, False, True),
            player_slot(2, True, "SandWave", "SA", (0.10, 0.42, 0.22, 1), 5, "Connecting", CONNECTING, False, False, True),
            player_slot(3, False, "", "", (0, 0, 0, 0), 0, "", READY_MUTED, False, False, False),
        ],
    )

    ready_btn = btn(
        "ReadyButton",
        "Ready",
        GOLD,
        GOLD_LABEL,
        label_size=32,
        amin=(0, 0.5),
        amax=(0, 0.5),
        pos=(40, 0),
        size=(340, 96),
        pivot=(0, 0.5),
        transition=0,
    )
    play_waiting = (0.18, 0.16, 0.14, 0.72)
    play_waiting_label = (0.62, 0.60, 0.56, 0.85)
    play_btn = Node(
        name="PlayButton",
        image=play_waiting,
        button=True,
        raycast=1,
        interactable=0,
        transition=0,
        amin=(1, 0.5),
        amax=(1, 0.5),
        pos=(-40, 0),
        size=(420, 96),
        pivot=(1, 0.5),
        children=[
            txt(
                "Label",
                "Waiting for Players...",
                26,
                play_waiting_label,
                4,
                amin=(0, 0),
                amax=(1, 1),
                pos=(0, 0),
                size=(0, 0),
            ),
            txt(
                "PlayLabel_StartMatch",
                "Start Match",
                30,
                GOLD_LABEL,
                4,
                active=0,
                amin=(0, 0),
                amax=(1, 1),
                pos=(0, 0),
                size=(0, 0),
            ),
        ],
    )
    footer = Node(
        name="FooterRoot",
        amin=(0.5, 0),
        amax=(0.5, 0),
        pos=(0, 36),
        size=(1480, 110),
        pivot=(0.5, 0),
        children=[ready_btn, play_btn],
    )

    def inactive_status(name: str, message: str) -> Node:
        return txt(
            name,
            message,
            26,
            MUTED,
            4,
            active=0,
            amin=(0, 0),
            amax=(1, 1),
            pos=(0, 0),
            size=(-48, -16),
        )

    lobby_status_panel = img(
        "LobbyStatusPanel",
        PANEL_BORDER,
        shadow=True,
        amin=(0.5, 0.5),
        amax=(0.5, 0.5),
        pos=(0, 8),
        size=(980, 78),
        children=[
            img("Fill", PANEL_BG, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(-6, -6)),
            txt(
                "LobbyStatusText",
                "Waiting for everyone to be Ready...",
                26,
                MUTED,
                4,
                amin=(0, 0),
                amax=(1, 1),
                pos=(0, 0),
                size=(-48, -16),
            ),
            inactive_status("StatusMsg_WaitingForPlayers", "Waiting for players..."),
            inactive_status("StatusMsg_PlayersJoining", "Players joining..."),
            inactive_status("StatusMsg_WaitingReady", "Waiting for everyone to be Ready..."),
            inactive_status("StatusMsg_ReadyToStart", "Ready to Start"),
        ],
    )
    countdown = txt(
        "CountdownPlaceholder",
        "Starting in: 00:10",
        28,
        GOLD_BRIGHT,
        4,
        active=0,
        amin=(0.5, 0),
        amax=(0.5, 0),
        pos=(0, -2),
        size=(420, 36),
        pivot=(0.5, 0),
    )
    status_root = Node(
        name="StatusRoot",
        amin=(0.5, 0),
        amax=(0.5, 0),
        pos=(0, 196),
        size=(1200, 100),
        pivot=(0.5, 0),
        children=[lobby_status_panel, countdown],
    )

    def inactive_system(name: str, message: str) -> Node:
        return txt(
            name,
            message,
            20,
            (MUTED[0], MUTED[1], MUTED[2], 0.75),
            4,
            active=0,
            amin=(0, 0),
            amax=(1, 1),
            pos=(0, 0),
            size=(0, 0),
            font_style=2,
        )

    message_footer = Node(
        name="MessageFooterRoot",
        amin=(0.5, 0),
        amax=(0.5, 0),
        pos=(0, 148),
        size=(1100, 40),
        pivot=(0.5, 0),
        children=[
            txt(
                "SystemMsg_PlayerJoined",
                "Player joined...",
                20,
                (MUTED[0], MUTED[1], MUTED[2], 0.75),
                4,
                amin=(0, 0),
                amax=(1, 1),
                pos=(0, 0),
                size=(0, 0),
                font_style=2,
            ),
            inactive_system("SystemMsg_PlayerLeft", "Player left..."),
            inactive_system("SystemMsg_HostChanged", "Host changed..."),
            inactive_system("SystemMsg_Searching", "Searching for player..."),
        ],
    )

    bg = img(
        "Background",
        WHITE,
        amin=(0, 0),
        amax=(1, 1),
        pos=(0, 0),
        size=(0, 0),
        sprite=BG_SPRITE,
    )
    safe = Node(name="SafeArea", amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(0, 0))

    canvas = Node(
        name="LobbyScreenCanvas",
        amin=(0, 0),
        amax=(0, 0),
        pos=(0, 0),
        size=(0, 0),
        pivot=(0, 0),
        scale=(0, 0, 0),
        children=[bg, safe, back, header, slots, status_root, message_footer, footer],
    )
    canvas.prep()
    slot0 = find_node(slots, "PlayerSlot_0")
    local_ready_status = find_node(slot0, "ReadyStatus") if slot0 else None
    local_ready_label = find_node(slot0, "ReadyLabel") if slot0 else None
    if local_ready_status is None or local_ready_label is None:
        raise RuntimeError("PlayerSlot_0 ReadyStatus/ReadyLabel missing for controller wiring.")
    # Canvas needs Canvas / Scaler / Raycaster / Controller — handle separately
    canvas_comp = nid()
    scaler_id = nid()
    ray_id = nid()
    ctrl_id = nid()

    lines: list[str] = [SCENE_HEADER.rstrip()]

    # Main Camera
    cam_go, cam_t, cam_c, cam_a = nid(), nid(), nid(), nid()
    lines += [
        f"--- !u!1 &{cam_go}",
        "GameObject:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        "  serializedVersion: 6",
        "  m_Component:",
        f"  - component: {{fileID: {cam_t}}}",
        f"  - component: {{fileID: {cam_c}}}",
        f"  - component: {{fileID: {cam_a}}}",
        "  m_Layer: 0",
        "  m_Name: Main Camera",
        "  m_TagString: MainCamera",
        "  m_Icon: {fileID: 0}",
        "  m_NavMeshLayer: 0",
        "  m_StaticEditorFlags: 0",
        "  m_IsActive: 1",
        f"--- !u!4 &{cam_t}",
        "Transform:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {cam_go}}}",
        "  serializedVersion: 2",
        "  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}",
        "  m_LocalPosition: {x: 0, y: 1, z: -10}",
        "  m_LocalScale: {x: 1, y: 1, z: 1}",
        "  m_ConstrainProportionsScale: 0",
        "  m_Children: []",
        "  m_Father: {fileID: 0}",
        "  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}",
        f"--- !u!20 &{cam_c}",
        "Camera:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {cam_go}}}",
        "  m_Enabled: 1",
        "  serializedVersion: 2",
        "  m_ClearFlags: 1",
        "  m_BackGroundColor: {r: 0.192, g: 0.212, b: 0.239, a: 0}",
        "  m_projectionMatrixMode: 1",
        "  m_GateFitMode: 2",
        "  m_FOVAxisMode: 0",
        "  m_Iso: 200",
        "  m_ShutterSpeed: 0.005",
        "  m_Aperture: 16",
        "  m_FocusDistance: 10",
        "  m_FocalLength: 50",
        "  m_BladeCount: 5",
        "  m_Curvature: {x: 2, y: 11}",
        "  m_BarrelClipping: 0.25",
        "  m_Anamorphism: 0",
        "  m_SensorSize: {x: 36, y: 24}",
        "  m_LensShift: {x: 0, y: 0}",
        "  m_NormalizedViewPortRect:",
        "    serializedVersion: 2",
        "    x: 0",
        "    y: 0",
        "    width: 1",
        "    height: 1",
        "  near clip plane: 0.3",
        "  far clip plane: 1000",
        "  field of view: 60",
        "  orthographic: 0",
        "  orthographic size: 5",
        "  m_Depth: -1",
        "  m_CullingMask:",
        "    serializedVersion: 2",
        "    m_Bits: 4294967295",
        "  m_RenderingPath: -1",
        "  m_TargetTexture: {fileID: 0}",
        "  m_TargetDisplay: 0",
        "  m_TargetEye: 3",
        "  m_HDR: 1",
        "  m_AllowMSAA: 1",
        "  m_AllowDynamicResolution: 0",
        "  m_ForceIntoRT: 0",
        "  m_OcclusionCulling: 1",
        "  m_StereoConvergence: 10",
        "  m_StereoSeparation: 0.022",
        f"--- !u!81 &{cam_a}",
        "AudioListener:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {cam_go}}}",
        "  m_Enabled: 1",
    ]

    # Emit canvas manually with extra components, then children
    canvas_comps = [canvas.rt, canvas_comp, scaler_id, ray_id, ctrl_id]
    lines.append(f"--- !u!1 &{canvas.go}")
    lines.append("GameObject:")
    lines.append("  m_ObjectHideFlags: 0")
    lines.append("  m_CorrespondingSourceObject: {fileID: 0}")
    lines.append("  m_PrefabInstance: {fileID: 0}")
    lines.append("  m_PrefabAsset: {fileID: 0}")
    lines.append("  serializedVersion: 6")
    lines.append("  m_Component:")
    for c in canvas_comps:
        lines.append(f"  - component: {{fileID: {c}}}")
    lines.append("  m_Layer: 0")
    lines.append("  m_Name: LobbyScreenCanvas")
    lines.append("  m_TagString: Untagged")
    lines.append("  m_Icon: {fileID: 0}")
    lines.append("  m_NavMeshLayer: 0")
    lines.append("  m_StaticEditorFlags: 0")
    lines.append("  m_IsActive: 1")
    lines.append(f"--- !u!224 &{canvas.rt}")
    lines.append("RectTransform:")
    lines.append("  m_ObjectHideFlags: 0")
    lines.append("  m_CorrespondingSourceObject: {fileID: 0}")
    lines.append("  m_PrefabInstance: {fileID: 0}")
    lines.append("  m_PrefabAsset: {fileID: 0}")
    lines.append(f"  m_GameObject: {{fileID: {canvas.go}}}")
    lines.append("  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}")
    lines.append("  m_LocalPosition: {x: 0, y: 0, z: 0}")
    lines.append("  m_LocalScale: {x: 0, y: 0, z: 0}")
    lines.append("  m_ConstrainProportionsScale: 0")
    lines.append("  m_Children:")
    for ch in canvas.children:
        lines.append(f"  - {{fileID: {ch.rt}}}")
    lines.append("  m_Father: {fileID: 0}")
    lines.append("  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}")
    lines.append("  m_AnchorMin: {x: 0, y: 0}")
    lines.append("  m_AnchorMax: {x: 0, y: 0}")
    lines.append("  m_AnchoredPosition: {x: 0, y: 0}")
    lines.append("  m_SizeDelta: {x: 0, y: 0}")
    lines.append("  m_Pivot: {x: 0, y: 0}")
    lines += [
        f"--- !u!223 &{canvas_comp}",
        "Canvas:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {canvas.go}}}",
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
        "  m_SortingOrder: 10",
        "  m_TargetDisplay: 0",
        f"--- !u!114 &{scaler_id}",
        "MonoBehaviour:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {canvas.go}}}",
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
        f"  m_GameObject: {{fileID: {canvas.go}}}",
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
        f"--- !u!114 &{ctrl_id}",
        "MonoBehaviour:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {canvas.go}}}",
        "  m_Enabled: 1",
        "  m_EditorHideFlags: 0",
        f"  m_Script: {{fileID: 11500000, guid: {GUID_CONTROLLER}, type: 3}}",
        "  m_Name: ",
        "  m_EditorClassIdentifier: ",
        f"  backButton: {{fileID: {back.btn_id}}}",
        f"  readyButton: {{fileID: {ready_btn.btn_id}}}",
        f"  readyButtonImage: {{fileID: {ready_btn.img_id}}}",
        f"  readyButtonLabel: {{fileID: {ready_btn.children[0].txt_id}}}",
        f"  localReadyStatus: {{fileID: {local_ready_status.img_id}}}",
        f"  localReadyLabel: {{fileID: {local_ready_label.txt_id}}}",
        f"  playButton: {{fileID: {play_btn.btn_id}}}",
        f"  playButtonImage: {{fileID: {play_btn.img_id}}}",
        f"  playButtonLabel: {{fileID: {play_btn.children[0].txt_id}}}",
    ]

    for ch in canvas.children:
        emit_node(lines, ch, canvas.rt)

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

    OUT.write_text("\n".join(lines) + "\n", encoding="utf-8")
    print(f"Wrote {OUT} ({len(lines)} lines)")


if __name__ == "__main__":
    main()
