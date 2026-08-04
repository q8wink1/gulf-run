#!/usr/bin/env python3
"""Patch Gameplay.unity with Sprint 23.13 OfflineRaceBootstrap."""

from __future__ import annotations

from pathlib import Path

ROOT = Path(r"C:\Projects\GulfRun")
SCENE = ROOT / "Client" / "Assets" / "_Project" / "Scenes" / "Gameplay.unity"

GUID_BOOTSTRAP = "b20c00000000000000000000000000f4"
RACE_MANAGER_MB = 540000003
PLAYER_MB = 510000137
CAMERA_MB = 510000148
SPAWN_MB = 530000003
TRACK_MB = 520000003

GO, TR, MB = 560000001, 560000002, 560000003
MARKER_START = "# --- SPRINT-23.13-OFFLINE-RACE ---"
MARKER_END = "# --- END SPRINT-23.13-OFFLINE-RACE ---"


def patch_scene() -> None:
    text = SCENE.read_text(encoding="utf-8")
    if MARKER_START in text:
        start = text.find(MARKER_START)
        end = text.find(MARKER_END)
        if start >= 0 and end > start:
            end = text.find("\n", end) + 1
            text = text[:start] + text[end:]

    block = f"""
{MARKER_START}
--- !u!1 &{GO}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {TR}}}
  - component: {{fileID: {MB}}}
  m_Layer: 0
  m_Name: OfflineRaceBootstrap
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{TR}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {GO}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: 0}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
--- !u!114 &{MB}
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {GO}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GUID_BOOTSTRAP}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  raceManager: {{fileID: {RACE_MANAGER_MB}}}
  playerController: {{fileID: {PLAYER_MB}}}
  cameraFollow: {{fileID: {CAMERA_MB}}}
  spawnManager: {{fileID: {SPAWN_MB}}}
  trackGenerator: {{fileID: {TRACK_MB}}}
  autoStartRace: 1
{MARKER_END}
"""
    text = text.rstrip() + "\n" + block + "\n"
    SCENE.write_text(text, encoding="utf-8")
    print(f"Patched {SCENE}")


if __name__ == "__main__":
    patch_scene()
