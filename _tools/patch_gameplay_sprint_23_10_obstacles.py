#!/usr/bin/env python3
"""Sprint 23.10 — lane obstacle markers, Kuwait profile, Gameplay scene hooks."""

from __future__ import annotations

import re
from pathlib import Path

ROOT = Path(r"C:\Projects\GulfRun")
PREFABS = ROOT / "Client" / "Assets" / "_Project" / "Prefabs" / "Track"
SCENE = ROOT / "Client" / "Assets" / "_Project" / "Scenes" / "Gameplay.unity"
KUWAIT = ROOT / "Client" / "Assets" / "_Project" / "Settings" / "SpawnProfiles" / "SpawnProfile_Kuwait.asset"

GUID_BRIDGE = "b20c00000000000000000000000000e6"
GUID_CATALOG_ASSET = "b20c00000000000000000000000000e1"


def marker_yaml(go: int, tr: int, mb: int, name: str, category: int, pos: tuple[float, float, float], lane: int) -> str:
    x, y, z = pos
    return f"""--- !u!1 &{go}
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
  m_Name: {name}
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
  m_LocalPosition: {{x: {x}, y: {y}, z: {z}}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 400000}}
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
  m_Script: {{fileID: 11500000, guid: b20c00000000000000000000000000c1, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  category: {category}
  lane: {lane}
  resolveLaneFromTransformX: 1
  laneSpacing: 2.2
  laneCenterX: 0
"""


def patch_track_prefab(path: Path) -> None:
    text = path.read_text(encoding="utf-8")

    # Root children: insert left/right obstacle transforms after existing obstacle tr.
    old_children = """  m_Children:
  - {fileID: 400010}
  - {fileID: 400020}
  - {fileID: 400030}
  - {fileID: 400040}
  - {fileID: 400050}
  - {fileID: 400060}
  - {fileID: 100101}
  - {fileID: 100111}
  - {fileID: 100121}
  - {fileID: 100131}
  - {fileID: 100141}"""
    new_children = """  m_Children:
  - {fileID: 400010}
  - {fileID: 400020}
  - {fileID: 400030}
  - {fileID: 400040}
  - {fileID: 400050}
  - {fileID: 400060}
  - {fileID: 100101}
  - {fileID: 100151}
  - {fileID: 100161}
  - {fileID: 100111}
  - {fileID: 100121}
  - {fileID: 100131}
  - {fileID: 100141}"""
    if "{fileID: 100151}" not in text:
        if old_children not in text:
            raise SystemExit(f"Children block not found in {path}")
        text = text.replace(old_children, new_children, 1)

    old_markers = """  spawnMarkers:
  - {fileID: 100102}
  - {fileID: 100112}
  - {fileID: 100122}
  - {fileID: 100132}
  - {fileID: 100142}"""
    new_markers = """  spawnMarkers:
  - {fileID: 100102}
  - {fileID: 100152}
  - {fileID: 100162}
  - {fileID: 100112}
  - {fileID: 100122}
  - {fileID: 100132}
  - {fileID: 100142}"""
    if "{fileID: 100152}" not in text:
        if old_markers not in text:
            raise SystemExit(f"spawnMarkers block not found in {path}")
        text = text.replace(old_markers, new_markers, 1)

    # Update center obstacle marker pose + lane fields.
    text = re.sub(
        r"(m_Name: SpawnMarker_Obstacle\n.*?m_LocalPosition: \{x: )[-\d.]+(, y: )[-\d.]+(, z: )[-\d.]+(\})",
        r"\g<1>0.0\g<2>0.5\g<3>16.0\g<4>",
        text,
        count=1,
        flags=re.S,
    )
    text = re.sub(
        r"(--- !u!114 &100102\nMonoBehaviour:.*?category: 0)(\n--- !u!1 &100110)",
        r"\1\n  lane: 1\n  resolveLaneFromTransformX: 1\n  laneSpacing: 2.2\n  laneCenterX: 0\2",
        text,
        count=1,
        flags=re.S,
    )

    if "SpawnMarker_Obstacle_Left" not in text:
        extra = marker_yaml(
            100150, 100151, 100152, "SpawnMarker_Obstacle_Left", 0, (-2.2, 0.5, 8.0), lane=0
        ) + marker_yaml(
            100160, 100161, 100162, "SpawnMarker_Obstacle_Right", 0, (2.2, 0.5, 28.0), lane=2
        )
        # Append before end of file (after last marker / content).
        text = text.rstrip() + "\n" + extra

    path.write_text(text, encoding="utf-8")
    print(f"Patched {path}")


def patch_kuwait() -> None:
    text = KUWAIT.read_text(encoding="utf-8")
    text = re.sub(
        r"(- category: 0\n    enabled: 1\n    spawnProbability: )[\d.]+(\n    spawnDensity: )[\d.]+(\n    minimumSpacing: )[\d.]+(\n    maximumSpacing: )[\d.]+",
        r"\g<1>0.7\g<2>0.85\g<3>6.0\g<4>18.0",
        text,
        count=1,
    )
    KUWAIT.write_text(text, encoding="utf-8")
    print(f"Patched {KUWAIT}")


def patch_scene() -> None:
    text = SCENE.read_text(encoding="utf-8")

    # SpawnManager new fields after obstacleCatalog.
    spawn_old = (
        "  obstacleCatalog: {fileID: 11400000, guid: b20c00000000000000000000000000e1, type: 2}\n"
        "  trackGenerator: {fileID: 520000003}\n"
        "  randomSeed: 0\n"
        "  logPlans: 0\n"
    )
    spawn_new = (
        "  obstacleCatalog: {fileID: 11400000, guid: b20c00000000000000000000000000e1, type: 2}\n"
        "  obstacleDifficulty: 1\n"
        "  executeObstaclePlans: 1\n"
        "  trackGenerator: {fileID: 520000003}\n"
        "  randomSeed: 0\n"
        "  logPlans: 0\n"
    )
    if "executeObstaclePlans:" not in text:
        if spawn_old not in text:
            raise SystemExit("SpawnManager block not found for patching.")
        text = text.replace(spawn_old, spawn_new, 1)

    # RaceManager: add bridge ref + difficulty; add component on RaceManager GO.
    race_old = (
        "  obstacleCatalog: {fileID: 11400000, guid: b20c00000000000000000000000000e1, type: 2}\n"
        "  initialSpeed: 12\n"
    )
    race_new = (
        "  obstacleCatalog: {fileID: 11400000, guid: b20c00000000000000000000000000e1, type: 2}\n"
        "  obstacleGameplayBridge: {fileID: 540000004}\n"
        "  obstacleDifficulty: 1\n"
        "  initialSpeed: 12\n"
    )
    if "obstacleGameplayBridge:" not in text:
        if race_old not in text:
            raise SystemExit("RaceManager obstacleCatalog block not found.")
        text = text.replace(race_old, race_new, 1)

    # Add ObstacleGameplayBridge component to RaceManager GameObject.
    if "--- !u!114 &540000004" not in text:
        text = text.replace(
            "  m_Component:\n"
            "  - component: {fileID: 540000002}\n"
            "  - component: {fileID: 540000003}\n"
            "  m_Layer: 0\n"
            "  m_Name: RaceManager\n",
            "  m_Component:\n"
            "  - component: {fileID: 540000002}\n"
            "  - component: {fileID: 540000003}\n"
            "  - component: {fileID: 540000004}\n"
            "  m_Layer: 0\n"
            "  m_Name: RaceManager\n",
            1,
        )
        bridge = f"""--- !u!114 &540000004
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 540000001}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_BRIDGE}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  raceManager: {{fileID: 540000003}}
  animatorDriver: {{fileID: 510000138}}
  cameraShake: {{fileID: 510000149}}
  applyLightCameraShake: 1
  hitShakeIntensity: 0.18
  hitShakeDuration: 0.12
  onHitAnimation:
    m_PersistentCalls:
      m_Calls: []
  onCameraShake:
    m_PersistentCalls:
      m_Calls: []
  onSoundEffect:
    m_PersistentCalls:
      m_Calls: []
  onSpeedReduction:
    m_PersistentCalls:
      m_Calls: []
"""
        end = "# --- END SPRINT-23.8-RACE-MANAGER ---"
        if end not in text:
            raise SystemExit("END SPRINT-23.8 marker missing.")
        text = text.replace(end, bridge + end, 1)

    SCENE.write_text(text, encoding="utf-8")
    print(f"Patched {SCENE}")


def main() -> None:
    patch_track_prefab(PREFABS / "TrackSegment_A.prefab")
    patch_track_prefab(PREFABS / "TrackSegment_B.prefab")
    patch_kuwait()
    patch_scene()


if __name__ == "__main__":
    main()
