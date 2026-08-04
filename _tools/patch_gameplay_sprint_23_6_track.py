#!/usr/bin/env python3
"""Generate Sprint 23.6 track segment prefabs + patch Gameplay.unity."""

from __future__ import annotations

from pathlib import Path

ROOT = Path(r"C:\Projects\GulfRun")
PREFABS = ROOT / "Client" / "Assets" / "_Project" / "Prefabs" / "Track"
SETTINGS = ROOT / "Client" / "Assets" / "_Project" / "Settings"
SCENE = ROOT / "Client" / "Assets" / "_Project" / "Scenes" / "Gameplay.unity"

GUID_SEGMENT = "b20c00000000000000000000000000c2"
GUID_MARKER = "b20c00000000000000000000000000c1"
GUID_GENERATOR = "b20c00000000000000000000000000c4"
GUID_SET_SCRIPT = "b20c00000000000000000000000000c3"
GUID_SET_ASSET = "b20c00000000000000000000000000c7"
GUID_PREFAB_A = "b20c00000000000000000000000000c5"
GUID_PREFAB_B = "b20c00000000000000000000000000c6"
GUID_FOLDER = "b20c00000000000000000000000000c0"

CUBE = "{fileID: 10202, guid: 0000000000000000e000000000000000, type: 0}"
LIT = "{fileID: 10303, guid: 0000000000000000f000000000000000, type: 0}"

LENGTH = 40.0
WIDTH = 10.0


def meta_prefab(guid: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
PrefabImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def meta_folder(guid: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
folderAsset: yes
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def meta_asset(guid: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
NativeFormatImporter:
  externalObjects: {{}}
  mainObjectFileID: 11400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def mesh_renderer_block(go: int, mr: int) -> list[str]:
    return [
        f"--- !u!23 &{mr}",
        "MeshRenderer:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {go}}}",
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
        f"  - {LIT}",
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


def box_collider(go: int, col: int) -> list[str]:
    return [
        f"--- !u!65 &{col}",
        "BoxCollider:",
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
        "  serializedVersion: 3",
        "  m_Size: {x: 1, y: 1, z: 1}",
        "  m_Center: {x: 0, y: 0, z: 0}",
    ]


def cube_child(
    *,
    go: int,
    tr: int,
    mf: int,
    mr: int,
    parent: int,
    name: str,
    pos: tuple[float, float, float],
    scale: tuple[float, float, float],
    col: int | None = None,
) -> list[str]:
    comps = [tr, mf, mr] + ([col] if col is not None else [])
    lines = [
        f"--- !u!1 &{go}",
        "GameObject:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        "  serializedVersion: 6",
        "  m_Component:",
    ]
    for c in comps:
        lines.append(f"  - component: {{fileID: {c}}}")
    lines += [
        "  m_Layer: 0",
        f"  m_Name: {name}",
        "  m_TagString: Untagged",
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
        f"  m_LocalPosition: {{x: {pos[0]}, y: {pos[1]}, z: {pos[2]}}}",
        f"  m_LocalScale: {{x: {scale[0]}, y: {scale[1]}, z: {scale[2]}}}",
        "  m_ConstrainProportionsScale: 0",
        "  m_Children: []",
        f"  m_Father: {{fileID: {parent}}}",
        "  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}",
        f"--- !u!33 &{mf}",
        "MeshFilter:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {go}}}",
        f"  m_Mesh: {CUBE}",
    ]
    lines += mesh_renderer_block(go, mr)
    if col is not None:
        lines += box_collider(go, col)
    return lines


def empty_child(go: int, tr: int, parent: int, name: str, pos: tuple[float, float, float], script_guid: str | None = None, mb: int | None = None, category: int | None = None) -> list[str]:
    comps = [tr] + ([mb] if mb is not None else [])
    lines = [
        f"--- !u!1 &{go}",
        "GameObject:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        "  serializedVersion: 6",
        "  m_Component:",
    ]
    for c in comps:
        lines.append(f"  - component: {{fileID: {c}}}")
    lines += [
        "  m_Layer: 0",
        f"  m_Name: {name}",
        "  m_TagString: Untagged",
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
        f"  m_LocalPosition: {{x: {pos[0]}, y: {pos[1]}, z: {pos[2]}}}",
        "  m_LocalScale: {x: 1, y: 1, z: 1}",
        "  m_ConstrainProportionsScale: 0",
        "  m_Children: []",
        f"  m_Father: {{fileID: {parent}}}",
        "  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}",
    ]
    if mb is not None and script_guid is not None and category is not None:
        lines += [
            f"--- !u!114 &{mb}",
            "MonoBehaviour:",
            "  m_ObjectHideFlags: 0",
            "  m_CorrespondingSourceObject: {fileID: 0}",
            "  m_PrefabInstance: {fileID: 0}",
            "  m_PrefabAsset: {fileID: 0}",
            f"  m_GameObject: {{fileID: {go}}}",
            "  m_Enabled: 1",
            "  m_EditorHideFlags: 0",
            f"  m_Script: {{fileID: 11500000, guid: {script_guid}, type: 3}}",
            "  m_Name: ",
            "  m_EditorClassIdentifier: ",
            f"  category: {category}",
        ]
    return lines


def build_prefab(name: str, variant: str) -> str:
    """variant 'A' = center dashed feel; 'B' = side rails."""
    # IDs
    root_go, root_tr, root_mb = 100000, 400000, 500000
    entry_go, entry_tr = 100010, 400010
    exit_go, exit_tr = 100020, 400020
    ground_go, ground_tr, ground_mf, ground_mr, ground_col = 100030, 400030, 330030, 230030, 650030
    lane_l = (100040, 400040, 330040, 230040)
    lane_r = (100050, 400050, 330050, 230050)
    extra = (100060, 400060, 330060, 230060)  # center dash (A) or left rail (B)

    markers = [
        ("SpawnMarker_Obstacle", 0, (0.0, 0.5, 12.0)),
        ("SpawnMarker_Coin", 1, (2.2, 0.8, 18.0)),
        ("SpawnMarker_PowerUp", 2, (-2.2, 0.8, 24.0)),
        ("SpawnMarker_Decoration", 3, (4.5, 0.5, 10.0)),
        ("SpawnMarker_Npc", 5, (-4.5, 0.5, 30.0)),  # SpawnCategory.Npc = 5
    ]
    # marker IDs start at 100100
    mid = 100100
    marker_nodes = []
    marker_mb_ids = []
    for i, (mname, cat, pos) in enumerate(markers):
        go = mid + i * 10
        tr = mid + i * 10 + 1
        mb = mid + i * 10 + 2
        marker_nodes.append((go, tr, mb, mname, cat, pos))
        marker_mb_ids.append(mb)

    child_trs = [
        entry_tr,
        exit_tr,
        ground_tr,
        lane_l[1],
        lane_r[1],
        extra[1],
    ] + [n[1] for n in marker_nodes]

    lines: list[str] = [
        "%YAML 1.1",
        "%TAG !u! tag:unity3d.com,2011:",
        f"--- !u!1 &{root_go}",
        "GameObject:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        "  serializedVersion: 6",
        "  m_Component:",
        f"  - component: {{fileID: {root_tr}}}",
        f"  - component: {{fileID: {root_mb}}}",
        "  m_Layer: 0",
        f"  m_Name: {name}",
        "  m_TagString: Untagged",
        "  m_Icon: {fileID: 0}",
        "  m_NavMeshLayer: 0",
        "  m_StaticEditorFlags: 0",
        "  m_IsActive: 1",
        f"--- !u!4 &{root_tr}",
        "Transform:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {root_go}}}",
        "  serializedVersion: 2",
        "  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}",
        "  m_LocalPosition: {x: 0, y: 0, z: 0}",
        "  m_LocalScale: {x: 1, y: 1, z: 1}",
        "  m_ConstrainProportionsScale: 0",
        "  m_Children:",
    ]
    for tr in child_trs:
        lines.append(f"  - {{fileID: {tr}}}")
    lines += [
        "  m_Father: {fileID: 0}",
        "  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}",
        f"--- !u!114 &{root_mb}",
        "MonoBehaviour:",
        "  m_ObjectHideFlags: 0",
        "  m_CorrespondingSourceObject: {fileID: 0}",
        "  m_PrefabInstance: {fileID: 0}",
        "  m_PrefabAsset: {fileID: 0}",
        f"  m_GameObject: {{fileID: {root_go}}}",
        "  m_Enabled: 1",
        "  m_EditorHideFlags: 0",
        f"  m_Script: {{fileID: 11500000, guid: {GUID_SEGMENT}, type: 3}}",
        "  m_Name: ",
        "  m_EditorClassIdentifier: ",
        f"  length: {LENGTH}",
        f"  entryPoint: {{fileID: {entry_tr}}}",
        f"  exitPoint: {{fileID: {exit_tr}}}",
        "  spawnMarkers:",
    ]
    for mb in marker_mb_ids:
        lines.append(f"  - {{fileID: {mb}}}")

    lines += empty_child(entry_go, entry_tr, root_tr, "Entry", (0.0, 0.0, 0.0))
    lines += empty_child(exit_go, exit_tr, root_tr, "Exit", (0.0, 0.0, LENGTH))

    # Ground centered at z = length/2
    lines += cube_child(
        go=ground_go,
        tr=ground_tr,
        mf=ground_mf,
        mr=ground_mr,
        parent=root_tr,
        name="Ground",
        pos=(0.0, -0.05, LENGTH * 0.5),
        scale=(WIDTH, 0.1, LENGTH),
        col=ground_col,
    )

    # Lane markings (thin raised strips at lane separators ±1.1)
    lines += cube_child(
        go=lane_l[0],
        tr=lane_l[1],
        mf=lane_l[2],
        mr=lane_l[3],
        parent=root_tr,
        name="LaneMark_Left",
        pos=(-1.1, 0.01, LENGTH * 0.5),
        scale=(0.08, 0.02, LENGTH * 0.95),
    )
    lines += cube_child(
        go=lane_r[0],
        tr=lane_r[1],
        mf=lane_r[2],
        mr=lane_r[3],
        parent=root_tr,
        name="LaneMark_Right",
        pos=(1.1, 0.01, LENGTH * 0.5),
        scale=(0.08, 0.02, LENGTH * 0.95),
    )

    if variant == "A":
        lines += cube_child(
            go=extra[0],
            tr=extra[1],
            mf=extra[2],
            mr=extra[3],
            parent=root_tr,
            name="CenterDash",
            pos=(0.0, 0.015, LENGTH * 0.5),
            scale=(0.12, 0.02, LENGTH * 0.4),
        )
    else:
        lines += cube_child(
            go=extra[0],
            tr=extra[1],
            mf=extra[2],
            mr=extra[3],
            parent=root_tr,
            name="SideRail",
            pos=(-4.8, 0.25, LENGTH * 0.5),
            scale=(0.2, 0.5, LENGTH * 0.9),
        )

    for go, tr, mb, mname, cat, pos in marker_nodes:
        lines += empty_child(go, tr, root_tr, mname, pos, GUID_MARKER, mb, cat)

    return "\n".join(lines) + "\n"


def write_segment_set() -> None:
    content = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_SET_SCRIPT}, type: 3}}
  m_Name: DefaultTrackSegmentSet
  m_EditorClassIdentifier: 
  setId: Default
  segments:
  - Prefab: {{fileID: 100100000, guid: {GUID_PREFAB_A}, type: 3}}
    Weight: 1
  - Prefab: {{fileID: 100100000, guid: {GUID_PREFAB_B}, type: 3}}
    Weight: 1
"""
    path = SETTINGS / "DefaultTrackSegmentSet.asset"
    path.write_text(content, encoding="utf-8")
    (SETTINGS / "DefaultTrackSegmentSet.asset.meta").write_text(meta_asset(GUID_SET_ASSET), encoding="utf-8")
    print(f"Wrote {path}")


def patch_scene() -> None:
    text = SCENE.read_text(encoding="utf-8")
    if "SPRINT-23.6-ENDLESS-TRACK" in text:
        print("Scene already has 23.6 track; refreshing generator block.")
        # Strip previous block between markers if re-run
        start = text.find("# --- SPRINT-23.6-ENDLESS-TRACK ---")
        end = text.find("# --- END SPRINT-23.6-ENDLESS-TRACK ---")
        if start >= 0 and end > start:
            end = text.find("\n", end) + 1
            text = text[:start] + text[end:]

    # Disable RunnerGround — segments provide the floor.
    text = text.replace(
        "  m_Name: RunnerGround\n  m_TagString: Untagged\n  m_Icon: {fileID: 0}\n  m_NavMeshLayer: 0\n  m_StaticEditorFlags: 0\n  m_IsActive: 1",
        "  m_Name: RunnerGround\n  m_TagString: Untagged\n  m_Icon: {fileID: 0}\n  m_NavMeshLayer: 0\n  m_StaticEditorFlags: 0\n  m_IsActive: 0",
        1,
    )

    go, tr, mb = 520000001, 520000002, 520000003
    block = f"""
# --- SPRINT-23.6-ENDLESS-TRACK ---
--- !u!1 &{go}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {tr}}}
  - component: {{fileID: {mb}}}
  m_Layer: 0
  m_Name: EndlessTrackGenerator
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{tr}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 0}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
--- !u!114 &{mb}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {go}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_GENERATOR}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  segmentLength: 40
  activeSegments: 6
  spawnDistance: 80
  despawnDistance: 40
  segmentSet: {{fileID: 11400000, guid: {GUID_SET_ASSET}, type: 2}}
  followTarget: {{fileID: 510000132}}
  segmentParent: {{fileID: {tr}}}
  preloadPerPrefab: 3
# --- END SPRINT-23.6-ENDLESS-TRACK ---
"""
    text = text.rstrip() + "\n" + block + "\n"
    SCENE.write_text(text, encoding="utf-8")
    print(f"Patched {SCENE}")


def main() -> None:
    PREFABS.mkdir(parents=True, exist_ok=True)
    (PREFABS.parent / "Track.meta" if False else None)
    folder_meta = PREFABS.with_suffix(".meta")
    # Prefabs/Track.meta sits beside the folder
    track_meta = PREFABS.parent / "Track.meta"
    # Actually folder is Prefabs/Track — meta is Prefabs/Track.meta
    track_folder_meta = Path(str(PREFABS) + ".meta")
    track_folder_meta.write_text(meta_folder(GUID_FOLDER), encoding="utf-8")

    a = PREFABS / "TrackSegment_A.prefab"
    b = PREFABS / "TrackSegment_B.prefab"
    a.write_text(build_prefab("TrackSegment_A", "A"), encoding="utf-8")
    b.write_text(build_prefab("TrackSegment_B", "B"), encoding="utf-8")
    (PREFABS / "TrackSegment_A.prefab.meta").write_text(meta_prefab(GUID_PREFAB_A), encoding="utf-8")
    (PREFABS / "TrackSegment_B.prefab.meta").write_text(meta_prefab(GUID_PREFAB_B), encoding="utf-8")
    print(f"Wrote {a}")
    print(f"Wrote {b}")

    write_segment_set()
    patch_scene()


if __name__ == "__main__":
    main()
