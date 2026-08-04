#!/usr/bin/env python3
"""Sprint 23.12 — coin/gem markers, catalog wiring, HUD currency refs."""

from __future__ import annotations

import re
from pathlib import Path

ROOT = Path(r"C:\Projects\GulfRun")
PREFABS = ROOT / "Client" / "Assets" / "_Project" / "Prefabs" / "Track"
SCENE = ROOT / "Client" / "Assets" / "_Project" / "Scenes" / "Gameplay.unity"

GUID_COLLECTIBLE_CATALOG = "b20c00000000000000000000000000f6"
GUID_TRACK_MARKER = "b20c00000000000000000000000000c1"


def marker_yaml(
    go: int,
    tr: int,
    mb: int,
    name: str,
    category: int,
    pos: tuple[float, float, float],
    lane: int,
) -> str:
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
  m_Script: {{fileID: 11500000, guid: {GUID_TRACK_MARKER}, type: 3}}
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

    old_children = """  m_Children:
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
  - {fileID: 100171}
  - {fileID: 100181}
  - {fileID: 100191}
  - {fileID: 100121}
  - {fileID: 100131}
  - {fileID: 100141}"""
    if "{fileID: 100171}" not in text:
        if old_children not in text:
            raise SystemExit(f"Children block not found in {path}")
        text = text.replace(old_children, new_children, 1)

    old_markers = """  spawnMarkers:
  - {fileID: 100102}
  - {fileID: 100152}
  - {fileID: 100162}
  - {fileID: 100112}
  - {fileID: 100122}
  - {fileID: 100132}
  - {fileID: 100142}"""
    new_markers = """  spawnMarkers:
  - {fileID: 100102}
  - {fileID: 100152}
  - {fileID: 100162}
  - {fileID: 100112}
  - {fileID: 100172}
  - {fileID: 100182}
  - {fileID: 100192}
  - {fileID: 100122}
  - {fileID: 100132}
  - {fileID: 100142}"""
    if "{fileID: 100172}" not in text:
        if old_markers not in text:
            raise SystemExit(f"spawnMarkers block not found in {path}")
        text = text.replace(old_markers, new_markers, 1)

    # Existing coin marker → center lane line start.
    text = re.sub(
        r"(m_Name: SpawnMarker_Coin\n.*?m_LocalPosition: \{x: )[-\d.]+(, y: )[-\d.]+(, z: )[-\d.]+(\})",
        r"\g<1>0.0\g<2>0.8\g<3>10.0\g<4>",
        text,
        count=1,
        flags=re.S,
    )
    text = re.sub(
        r"(--- !u!114 &100112\nMonoBehaviour:.*?category: 1)(\n--- !u!1 &100120)",
        r"\1\n  lane: 1\n  resolveLaneFromTransformX: 1\n  laneSpacing: 2.2\n  laneCenterX: 0\2",
        text,
        count=1,
        flags=re.S,
    )

    if "SpawnMarker_Coin_Left" not in text:
        extra = (
            marker_yaml(
                100170, 100171, 100172, "SpawnMarker_Coin_Left", 1, (-2.2, 0.8, 14.0), lane=0
            )
            + marker_yaml(
                100180, 100181, 100182, "SpawnMarker_Coin_Right", 1, (2.2, 0.8, 22.0), lane=2
            )
            + marker_yaml(
                100190, 100191, 100192, "SpawnMarker_Gem", 6, (0.0, 0.9, 32.0), lane=1
            )
        )
        text = text.rstrip() + "\n" + extra

    path.write_text(text, encoding="utf-8")
    print(f"Patched {path}")


def patch_scene() -> None:
    text = SCENE.read_text(encoding="utf-8")

    spawn_old = (
        "  obstacleCatalog: {fileID: 11400000, guid: b20c00000000000000000000000000e1, type: 2}\n"
        "  obstacleDifficulty: 1\n"
        "  executeObstaclePlans: 1\n"
        "  trackGenerator: {fileID: 520000003}\n"
        "  randomSeed: 0\n"
        "  logPlans: 0\n"
    )
    spawn_new = (
        "  obstacleCatalog: {fileID: 11400000, guid: b20c00000000000000000000000000e1, type: 2}\n"
        "  obstacleDifficulty: 1\n"
        "  executeObstaclePlans: 1\n"
        f"  collectibleCatalog: {{fileID: 11400000, guid: {GUID_COLLECTIBLE_CATALOG}, type: 2}}\n"
        "  executeCollectiblePlans: 1\n"
        "  defaultCoinPattern: 1\n"
        "  randomizeCoinPattern: 1\n"
        "  allowGemPatterns: 0\n"
        "  lineCount: 5\n"
        "  lineSpacingZ: 1.4\n"
        "  arcHeight: 0.55\n"
        "  laneSpacing: 2.2\n"
        "  laneCenterX: 0\n"
        "  trackGenerator: {fileID: 520000003}\n"
        "  randomSeed: 0\n"
        "  logPlans: 0\n"
    )
    if "collectibleCatalog:" not in text:
        if spawn_old not in text:
            raise SystemExit("SpawnManager block not found for collectible patch.")
        text = text.replace(spawn_old, spawn_new, 1)

    hud_old = (
        "  pauseButton: {fileID: 510000106}\n"
        "  resumeButton: {fileID: 510000125}\n"
        "  pauseMenuPanel: {fileID: 510000088}\n"
        "  notificationRoot: {fileID: 510000068}\n"
        "  notificationText: {fileID: 510000117}\n"
        "  notificationBackground: {fileID: 510000114}\n"
        "  playNotificationDemo: 1\n"
        "  notificationDemoIntervalSeconds: 4.5\n"
        "  notificationVisibleSeconds: 1.6\n"
    )
    hud_new = (
        "  pauseButton: {fileID: 510000106}\n"
        "  resumeButton: {fileID: 510000125}\n"
        "  pauseMenuPanel: {fileID: 510000088}\n"
        "  notificationRoot: {fileID: 510000068}\n"
        "  notificationText: {fileID: 510000117}\n"
        "  notificationBackground: {fileID: 510000114}\n"
        "  coinsText: {fileID: 510000103}\n"
        "  gemsText: {fileID: 510000104}\n"
        "  playNotificationDemo: 1\n"
        "  notificationDemoIntervalSeconds: 4.5\n"
        "  notificationVisibleSeconds: 1.6\n"
    )
    if "coinsText:" not in text:
        if hud_old not in text:
            raise SystemExit("GameplayHudController block not found for currency patch.")
        text = text.replace(hud_old, hud_new, 1)

    text = text.replace("  m_Text: COINS  42\n", "  m_Text: COINS  0\n", 1)
    text = text.replace("  m_Text: GEMS  7\n", "  m_Text: GEMS  0\n", 1)
    text = text.replace("  m_Text: GEMS  3\n", "  m_Text: GEMS  0\n", 1)

    SCENE.write_text(text, encoding="utf-8")
    print(f"Patched {SCENE}")


def main() -> None:
    patch_track_prefab(PREFABS / "TrackSegment_A.prefab")
    patch_track_prefab(PREFABS / "TrackSegment_B.prefab")
    patch_scene()


if __name__ == "__main__":
    main()
