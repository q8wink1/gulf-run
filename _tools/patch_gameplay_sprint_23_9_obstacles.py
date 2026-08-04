#!/usr/bin/env python3
"""Sprint 23.9 — obstacle sample assets, placeholder prefab, Gameplay scene hooks."""

from __future__ import annotations

from pathlib import Path

ROOT = Path(r"C:\Projects\GulfRun")
SETTINGS = ROOT / "Client" / "Assets" / "_Project" / "Settings" / "Obstacles"
PREFABS = ROOT / "Client" / "Assets" / "_Project" / "Prefabs" / "Track"
SCENE = ROOT / "Client" / "Assets" / "_Project" / "Scenes" / "Gameplay.unity"

GUID_OBSTACLE_DATA = "b20c00000000000000000000000000d5"
GUID_STATIC_OBSTACLE = "b20c00000000000000000000000000d8"
GUID_CATALOG = "b20c00000000000000000000000000dc"

GUID_FOLDER = "b20c00000000000000000000000000dd"
GUID_DATA_STATIC = "b20c00000000000000000000000000de"
GUID_DATA_SLIDE = "b20c00000000000000000000000000df"
GUID_DATA_JUMP = "b20c00000000000000000000000000e0"
GUID_CATALOG_ASSET = "b20c00000000000000000000000000e1"
GUID_PREFAB = "b20c00000000000000000000000000e2"
GUID_PREFAB_SLIDE = "b20c00000000000000000000000000e3"
GUID_PREFAB_JUMP = "b20c00000000000000000000000000e4"
GUID_SLIDE_SCRIPT = "b20c00000000000000000000000000db"
GUID_JUMP_SCRIPT = "b20c00000000000000000000000000da"


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


def meta_prefab(guid: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
NativeFormatImporter:
  externalObjects: {{}}
  mainObjectFileID: 100000
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


def write_obstacle_data(
    filename: str,
    guid: str,
    display_name: str,
    obstacle_type: int,
    width: float,
    height: float,
    difficulty: int,
    spawn_weight: float,
    required_action: int,
) -> None:
    body = f"""%YAML 1.1
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
  m_Script: {{fileID: 11500000, guid: {GUID_OBSTACLE_DATA}, type: 3}}
  m_Name: {filename}
  m_EditorClassIdentifier: 
  displayName: {display_name}
  obstacleType: {obstacle_type}
  width: {width}
  height: {height}
  difficulty: {difficulty}
  spawnWeight: {spawn_weight}
  requiredAction: {required_action}
"""
    path = SETTINGS / f"{filename}.asset"
    path.write_text(body, encoding="utf-8")
    path.with_suffix(".asset.meta").write_text(meta_asset(guid), encoding="utf-8")
    print(f"Wrote {path}")


def write_obstacle_prefab(
    filename: str,
    prefab_guid: str,
    script_guid: str,
    data_guid: str,
    visual_y: float,
    visual_scale_y: float,
    collider_center_y: float,
    collider_size_y: float,
) -> None:
    body = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1 &100000
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 400000}}
  - component: {{fileID: 650000}}
  - component: {{fileID: 114000}}
  m_Layer: 0
  m_Name: {filename}
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &400000
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 100000}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: 400010}}
  m_Father: {{fileID: 0}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
--- !u!65 &650000
BoxCollider:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 100000}}
  m_Material: {{fileID: 0}}
  m_IncludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_ExcludeLayers:
    serializedVersion: 2
    m_Bits: 0
  m_LayerOverridePriority: 0
  m_IsTrigger: 1
  m_ProvidesContacts: 0
  m_Enabled: 1
  serializedVersion: 3
  m_Size: {{x: 1.2, y: {collider_size_y}, z: 0.8}}
  m_Center: {{x: 0, y: {collider_center_y}, z: 0}}
--- !u!114 &114000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 100000}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {script_guid}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  data: {{fileID: 11400000, guid: {data_guid}, type: 2}}
  lane: 1
  placementEulerAngles: {{x: 0, y: 0, z: 0}}
  placementScale: {{x: 1, y: 1, z: 1}}
  obstacleEnabled: 1
  laneSpacing: 2.2
  laneCenterX: 0
  obstacleCollider: {{fileID: 650000}}
  visualModel: {{fileID: 400010}}
--- !u!1 &100010
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: 400010}}
  - component: {{fileID: 330010}}
  - component: {{fileID: 230010}}
  m_Layer: 0
  m_Name: Visual
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &400010
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 100010}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: {visual_y}, z: 0}}
  m_LocalScale: {{x: 1.2, y: {visual_scale_y}, z: 0.8}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 400000}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
--- !u!33 &330010
MeshFilter:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 100010}}
  m_Mesh: {{fileID: 10202, guid: 0000000000000000e000000000000000, type: 0}}
--- !u!23 &230010
MeshRenderer:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 100010}}
  m_Enabled: 1
  m_CastShadows: 1
  m_ReceiveShadows: 1
  m_DynamicOccludee: 1
  m_StaticShadowCaster: 0
  m_MotionVectors: 1
  m_LightProbeUsage: 1
  m_ReflectionProbeUsage: 1
  m_RayTracingMode: 2
  m_RayTraceProcedural: 0
  m_RenderingLayerMask: 1
  m_RendererPriority: 0
  m_Materials:
  - {{fileID: 10303, guid: 0000000000000000f000000000000000, type: 0}}
  m_StaticBatchInfo:
    firstSubMesh: 0
    subMeshCount: 0
  m_StaticBatchRoot: {{fileID: 0}}
  m_ProbeAnchor: {{fileID: 0}}
  m_LightProbeVolumeOverride: {{fileID: 0}}
  m_ScaleInLightmap: 1
  m_ReceiveGI: 1
  m_PreserveUVs: 0
  m_IgnoreNormalsForChartDetection: 0
  m_ImportantGI: 0
  m_StitchLightmapSeams: 1
  m_SelectedEditorRenderState: 3
  m_MinimumChartSize: 4
  m_AutoUVMaxDistance: 0.5
  m_AutoUVMaxAngle: 89
  m_LightmapParameters: {{fileID: 0}}
  m_SortingLayerID: 0
  m_SortingLayer: 0
  m_SortingOrder: 0
  m_AdditionalVertexStreams: {{fileID: 0}}
"""
    path = PREFABS / f"{filename}.prefab"
    path.write_text(body, encoding="utf-8")
    path.with_suffix(".prefab.meta").write_text(meta_prefab(prefab_guid), encoding="utf-8")
    print(f"Wrote {path}")


def write_catalog() -> None:
    body = f"""%YAML 1.1
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
  m_Script: {{fileID: 11500000, guid: {GUID_CATALOG}, type: 3}}
  m_Name: ObstacleCatalog_Default
  m_EditorClassIdentifier: 
  entries:
  - data: {{fileID: 11400000, guid: {GUID_DATA_STATIC}, type: 2}}
    prefab: {{fileID: 100000, guid: {GUID_PREFAB}, type: 3}}
    preloadCount: 4
  - data: {{fileID: 11400000, guid: {GUID_DATA_SLIDE}, type: 2}}
    prefab: {{fileID: 100000, guid: {GUID_PREFAB_SLIDE}, type: 3}}
    preloadCount: 2
  - data: {{fileID: 11400000, guid: {GUID_DATA_JUMP}, type: 2}}
    prefab: {{fileID: 100000, guid: {GUID_PREFAB_JUMP}, type: 3}}
    preloadCount: 2
"""
    path = SETTINGS / "ObstacleCatalog_Default.asset"
    path.write_text(body, encoding="utf-8")
    path.with_suffix(".asset.meta").write_text(meta_asset(GUID_CATALOG_ASSET), encoding="utf-8")
    print(f"Wrote {path}")


def patch_scene() -> None:
    text = SCENE.read_text(encoding="utf-8")

    # SpawnManager: insert obstacleCatalog after spawnProfile line if missing.
    spawn_needle = "  spawnProfile: {fileID: 11400000, guid: b20c00000000000000000000000000cd, type: 2}\n  trackGenerator:"
    spawn_repl = (
        "  spawnProfile: {fileID: 11400000, guid: b20c00000000000000000000000000cd, type: 2}\n"
        f"  obstacleCatalog: {{fileID: 11400000, guid: {GUID_CATALOG_ASSET}, type: 2}}\n"
        "  trackGenerator:"
    )
    if "obstacleCatalog:" not in text.split("# --- SPRINT-23.7-SPAWN-MANAGER ---")[1].split(
        "# --- END SPRINT-23.7-SPAWN-MANAGER ---"
    )[0]:
        if spawn_needle not in text:
            raise SystemExit("SpawnManager spawnProfile block not found for patching.")
        text = text.replace(spawn_needle, spawn_repl, 1)
    else:
        # Refresh catalog guid if already present.
        import re

        text = re.sub(
            r"(# --- SPRINT-23\.7-SPAWN-MANAGER ---.*?obstacleCatalog: \{fileID: 11400000, guid: )[0-9a-f]+(, type: 2\})",
            rf"\g<1>{GUID_CATALOG_ASSET}\2",
            text,
            count=1,
            flags=re.S,
        )

    # RaceManager: insert obstacleCatalog after hud line.
    race_needle = "  hud: {fileID: 510000006}\n  initialSpeed:"
    race_repl = (
        "  hud: {fileID: 510000006}\n"
        f"  obstacleCatalog: {{fileID: 11400000, guid: {GUID_CATALOG_ASSET}, type: 2}}\n"
        "  initialSpeed:"
    )
    race_block = text.split("# --- SPRINT-23.8-RACE-MANAGER ---")[1].split(
        "# --- END SPRINT-23.8-RACE-MANAGER ---"
    )[0]
    if "obstacleCatalog:" not in race_block:
        if race_needle not in text:
            raise SystemExit("RaceManager hud block not found for patching.")
        text = text.replace(race_needle, race_repl, 1)

    SCENE.write_text(text, encoding="utf-8")
    print(f"Patched {SCENE}")


def main() -> None:
    SETTINGS.mkdir(parents=True, exist_ok=True)
    (Path(str(SETTINGS) + ".meta")).write_text(meta_folder(GUID_FOLDER), encoding="utf-8")

    # ObstacleType: Static=0 Moving=1 Jump=2 Slide=3
    # RequiredAction: None=0 Jump=1 Slide=2 SwitchLane=3
    write_obstacle_data(
        "ObstacleData_StaticBarrier",
        GUID_DATA_STATIC,
        "Static Barrier",
        obstacle_type=0,
        width=1.2,
        height=1.5,
        difficulty=1,
        spawn_weight=1.0,
        required_action=3,
    )
    write_obstacle_data(
        "ObstacleData_LowBeam",
        GUID_DATA_SLIDE,
        "Low Beam",
        obstacle_type=3,
        width=1.4,
        height=0.9,
        difficulty=2,
        spawn_weight=0.8,
        required_action=2,
    )
    write_obstacle_data(
        "ObstacleData_Curb",
        GUID_DATA_JUMP,
        "Curb",
        obstacle_type=2,
        width=1.2,
        height=0.7,
        difficulty=1,
        spawn_weight=0.9,
        required_action=1,
    )

    write_obstacle_prefab(
        "Obstacle_Static_Placeholder",
        GUID_PREFAB,
        GUID_STATIC_OBSTACLE,
        GUID_DATA_STATIC,
        visual_y=0.75,
        visual_scale_y=1.5,
        collider_center_y=0.75,
        collider_size_y=1.5,
    )
    write_obstacle_prefab(
        "Obstacle_Slide_Placeholder",
        GUID_PREFAB_SLIDE,
        GUID_SLIDE_SCRIPT,
        GUID_DATA_SLIDE,
        visual_y=1.4,
        visual_scale_y=0.6,
        collider_center_y=1.4,
        collider_size_y=0.6,
    )
    write_obstacle_prefab(
        "Obstacle_Jump_Placeholder",
        GUID_PREFAB_JUMP,
        GUID_JUMP_SCRIPT,
        GUID_DATA_JUMP,
        visual_y=0.35,
        visual_scale_y=0.7,
        collider_center_y=0.35,
        collider_size_y=0.7,
    )

    write_catalog()
    patch_scene()


if __name__ == "__main__":
    main()
