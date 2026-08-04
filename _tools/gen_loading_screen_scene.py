#!/usr/bin/env python3
"""Generate LoadingScreen.unity (Sprint 22.5 Premium Loading Screen UI) without Unity batchmode."""

from __future__ import annotations

from dataclasses import dataclass, field
from pathlib import Path

OUT = Path(r"C:\Projects\GulfRun\Client\Assets\_Project\Scenes\LoadingScreen.unity")

GUID_TEXT = "5f7201a12d95ffc409449d95f23cf332"
GUID_IMAGE = "fe87c0e1cc204ed48ad3b37840f39efc"
GUID_BUTTON = "4e29b1a8efbd4b44bb3f3716e73f07ff"
GUID_SCALER = "0cd44c1031e13a943bb63640046fad76"
GUID_RAYCASTER = "dc42784cf147c0c48a680349fa168899"
GUID_SHADOW = "cfabb0440166ab443bba8876756fdfa9"
GUID_EVENTSYSTEM = "76c392e42b5098c458856cdf6ecaaaa1"
GUID_STANDALONE = "4f231c4fb786f3946a6b90b886c48677"
GUID_CONTROLLER = "b20c0000000000000000000000000093"
GUID_BG = "a18b0000000000000000000000000001"
GUID_LOGO = "a18c1000000000000000000000000001"
FONT = "{fileID: 10102, guid: 0000000000000000e000000000000000, type: 0}"
BG_SPRITE = f"{{fileID: 21300000, guid: {GUID_BG}, type: 3}}"
LOGO_SPRITE = f"{{fileID: 21300000, guid: {GUID_LOGO}, type: 3}}"

GOLD = (0.90, 0.71, 0.25, 1.0)
GOLD_BRIGHT = (1.0, 0.84, 0.40, 1.0)
DIM = (0.02, 0.02, 0.04, 0.62)
MAP_WASH = (0.55, 0.38, 0.22, 0.35)
PANEL_BG = (0.10, 0.09, 0.10, 0.78)
PANEL_BORDER = (0.90, 0.71, 0.25, 0.55)
WHITE = (1.0, 1.0, 1.0, 1.0)
MUTED = (0.80, 0.80, 0.80, 1.0)
GOLD_LABEL = (0.20, 0.14, 0.02, 1.0)
SUCCESS = (0.40, 0.85, 0.45, 1.0)
BG_TINT = (0.85, 0.85, 0.88, 1.0)

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
    image: tuple[float, float, float, float] | None = None
    sprite: str = "{fileID: 0}"
    preserve: int = 0
    raycast: int = 0
    shadow: bool = False
    text: str | None = None
    font_size: int = 24
    font_style: int = 1
    align: int = 4
    text_color: tuple[float, float, float, float] = WHITE
    button: bool = False
    interactable: int = 1
    transition: int = 1
    img_type: int = 0
    fill_amount: float = 1.0
    fill_method: int = 4
    fill_origin: int = 0
    script_guid: str | None = None
    cr: int = field(default_factory=nid)
    img_id: int | None = None
    txt_id: int | None = None
    btn_id: int | None = None
    sh_id: int | None = None
    script_id: int | None = None

    def prep(self) -> None:
        if self.image is not None or self.button:
            self.img_id = nid()
        if self.text is not None:
            self.txt_id = nid()
        if self.button:
            self.btn_id = nid()
        if self.shadow:
            self.sh_id = nid()
        if self.script_guid:
            self.script_id = nid()
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
    if n.script_id:
        out.append(n.script_id)
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
        lines.append(f"  m_Color: {c4(n.image if n.image is not None else WHITE)}")
        lines.append(f"  m_RaycastTarget: {n.raycast}")
        lines.append("  m_RaycastPadding: {x: 0, y: 0, z: 0, w: 0}")
        lines.append("  m_Maskable: 1")
        lines.append("  m_OnCullStateChanged:")
        lines.append("    m_PersistentCalls:")
        lines.append("      m_Calls: []")
        lines.append(f"  m_Sprite: {n.sprite}")
        lines.append(f"  m_Type: {n.img_type}")
        lines.append(f"  m_PreserveAspect: {n.preserve}")
        lines.append("  m_FillCenter: 1")
        lines.append(f"  m_FillMethod: {n.fill_method}")
        lines.append(f"  m_FillAmount: {n.fill_amount}")
        lines.append("  m_FillClockwise: 1")
        lines.append(f"  m_FillOrigin: {n.fill_origin}")
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
        lines.append("    m_Font: " + FONT)
        lines.append(f"    m_FontSize: {n.font_size}")
        lines.append(f"    m_FontStyle: {n.font_style}")
        lines.append("    m_BestFit: 0")
        lines.append("    m_MinSize: 10")
        lines.append("    m_MaxSize: 40")
        lines.append(f"    m_Alignment: {n.align}")
        lines.append("    m_AlignByGeometry: 0")
        lines.append("    m_RichText: 1")
        lines.append("    m_HorizontalOverflow: 0")
        lines.append("    m_VerticalOverflow: 0")
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
        lines.append("    m_DisabledColor: {r: 0.78, g: 0.78, b: 0.78, a: 0.5}")
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
        lines.append("  m_EffectColor: {r: 0, g: 0, b: 0, a: 0.42}")
        lines.append("  m_EffectDistance: {x: 0, y: -6}")
        lines.append("  m_UseGraphicAlpha: 1")

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

    bg = img(
        "Background",
        BG_TINT,
        amin=(0, 0),
        amax=(1, 1),
        pos=(0, 0),
        size=(0, 0),
        sprite=BG_SPRITE,
    )
    map_wash = img(
        "MapBlurPlaceholder",
        MAP_WASH,
        amin=(0, 0),
        amax=(1, 1),
        pos=(0, 0),
        size=(0, 0),
    )
    dim = img(
        "DimOverlay",
        DIM,
        amin=(0, 0),
        amax=(1, 1),
        pos=(0, 0),
        size=(0, 0),
    )
    safe = Node(name="SafeArea", amin=(0, 0), amax=(1, 1), pos=(0, -9), size=(-96, -86))

    center = Node(
        name="CenterRoot",
        amin=(0.5, 0.5),
        amax=(0.5, 0.5),
        pos=(0, 120),
        size=(720, 420),
        children=[
            img(
                "Logo",
                WHITE,
                amin=(0.5, 1),
                amax=(0.5, 1),
                pivot=(0.5, 1),
                pos=(0, 0),
                size=(420, 220),
                sprite=LOGO_SPRITE,
                preserve=1,
            ),
            txt(
                "LoadingText",
                "Loading Race...",
                40,
                GOLD_BRIGHT,
                4,
                amin=(0, 0.18),
                amax=(1, 0.38),
                pos=(0, 0),
                size=(-32, 0),
            ),
            img(
                "Spinner",
                GOLD_BRIGHT,
                amin=(0.5, 0),
                amax=(0.5, 0),
                pivot=(0.5, 0.5),
                pos=(0, 36),
                size=(72, 72),
            ),
        ],
    )

    progress = Node(
        name="ProgressRoot",
        amin=(0.5, 0.5),
        amax=(0.5, 0.5),
        pos=(0, -80),
        size=(760, 72),
        image=PANEL_BORDER,
        shadow=True,
        children=[
            img("Track", PANEL_BG, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(-8, -8)),
            img(
                "Fill",
                GOLD,
                amin=(0, 0),
                amax=(1, 1),
                pos=(-56, 0),
                size=(-128, -28),
                img_type=1,
                fill_amount=0.0,
                fill_method=0,
                fill_origin=0,
            ),
            txt(
                "PercentText",
                "0%",
                26,
                WHITE,
                5,
                amin=(1, 0),
                amax=(1, 1),
                pivot=(1, 0.5),
                pos=(-16, 0),
                size=(96, 0),
            ),
        ],
    )

    tips = Node(
        name="TipsPanel",
        amin=(0.5, 0),
        amax=(0.5, 0),
        pivot=(0.5, 0),
        pos=(0, 168),
        size=(980, 168),
        image=PANEL_BORDER,
        shadow=True,
        children=[
            img("Fill", PANEL_BG, amin=(0, 0), amax=(1, 1), pos=(0, 0), size=(-6, -6)),
            txt(
                "TipsTitle",
                "TIP",
                20,
                GOLD,
                3,
                amin=(0, 1),
                amax=(1, 1),
                pivot=(0.5, 1),
                pos=(0, -14),
                size=(-48, 28),
            ),
            txt(
                "TipPrimary",
                "Grab item boxes mid-race to turn the tide.",
                26,
                WHITE,
                3,
                amin=(0, 0),
                amax=(1, 1),
                pos=(0, -6),
                size=(-56, -84),
            ),
            txt(
                "TipSecondary",
                "Jump early to clear desert traps.",
                22,
                MUTED,
                3,
                amin=(0, 0),
                amax=(1, 1),
                pos=(0, 0),
                size=(-56, -56),
                active=0,
                font_style=0,
            ),
            txt(
                "TipTertiary",
                "Draft behind rivals to save your boost.",
                22,
                MUTED,
                3,
                amin=(0, 0),
                amax=(1, 1),
                pos=(0, 0),
                size=(-56, -56),
                active=0,
                font_style=0,
            ),
        ],
    )

    sync = Node(
        name="SyncStatusRoot",
        amin=(0.5, 0),
        amax=(0.5, 0),
        pivot=(0.5, 0),
        pos=(0, 48),
        size=(720, 88),
        children=[
            txt(
                "SyncStatusText",
                "Waiting for players...",
                24,
                MUTED,
                4,
                amin=(0, 0.45),
                amax=(1, 1),
                pos=(0, 0),
                size=(-16, 0),
            ),
            txt(
                "ReadyCountText",
                "4 / 4 Ready",
                28,
                SUCCESS,
                4,
                amin=(0, 0),
                amax=(1, 0.55),
                pos=(0, 0),
                size=(-16, 0),
            ),
        ],
    )

    continue_btn = btn(
        "ContinueButton",
        "Continue",
        GOLD,
        GOLD_LABEL,
        amin=(1, 0),
        amax=(1, 0),
        pos=(-48, 48),
        size=(280, 72),
        pivot=(1, 0),
    )

    canvas = Node(
        name="LoadingScreenCanvas",
        amin=(0, 0),
        amax=(0, 0),
        pos=(0, 0),
        size=(0, 0),
        pivot=(0, 0),
        scale=(0, 0, 0),
        children=[bg, map_wash, dim, safe, center, progress, tips, sync, continue_btn],
    )
    canvas.prep()

    spinner = find_node(center, "Spinner")
    if spinner is None:
        raise RuntimeError("Spinner missing")
    if continue_btn.btn_id is None:
        raise RuntimeError("ContinueButton missing")

    canvas_comp = nid()
    scaler_id = nid()
    ray_id = nid()
    ctrl_id = nid()

    lines: list[str] = [SCENE_HEADER.rstrip()]

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
    lines.append("  m_Name: LoadingScreenCanvas")
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
        f"  continueButton: {{fileID: {continue_btn.btn_id}}}",
        f"  spinner: {{fileID: {spinner.rt}}}",
        "  spinnerDegreesPerSecond: 180",
    ]

    for ch in canvas.children:
        emit_node(lines, ch, canvas.rt)

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

    # Roots: Main Camera, Canvas, EventSystem
    lines += [
        "--- !u!1660057539 &9223372036854775807",
        "SceneRoots:",
        "  m_ObjectHideFlags: 0",
        f"  m_Roots:",
        f"  - {{fileID: {cam_t}}}",
        f"  - {{fileID: {canvas.rt}}}",
        f"  - {{fileID: {es_t}}}",
    ]

    OUT.write_text("\n".join(lines) + "\n", encoding="utf-8")
    print(f"Wrote {OUT} ({len(lines)} lines)")


if __name__ == "__main__":
    main()
