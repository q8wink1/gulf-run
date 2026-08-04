#!/usr/bin/env python3
"""Patch Gameplay.unity with Sprint 23.8 RaceManager (serialized refs)."""

from __future__ import annotations

from pathlib import Path

ROOT = Path(r"C:\Projects\GulfRun")
SCENE = ROOT / "Client" / "Assets" / "_Project" / "Scenes" / "Gameplay.unity"

GUID_RACE_MANAGER = "b20c00000000000000000000000000d2"

# Existing MonoBehaviour fileIDs in Gameplay.unity
PLAYER = 510000137
CAMERA = 510000148
TRACK = 520000003
SPAWN = 530000003
HUD = 510000006

GO, TR, MB = 540000001, 540000002, 540000003
MARKER_START = "# --- SPRINT-23.8-RACE-MANAGER ---"
MARKER_END = "# --- END SPRINT-23.8-RACE-MANAGER ---"


def unity_event_empty(indent: str = "  ") -> str:
    return f"""{indent}m_PersistentCalls:
{indent}  m_Calls: []
"""


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
  m_Name: RaceManager
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
  m_Script: {{fileID: 11500000, guid: {GUID_RACE_MANAGER}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  playerController: {{fileID: {PLAYER}}}
  cameraController: {{fileID: {CAMERA}}}
  trackGenerator: {{fileID: {TRACK}}}
  spawnManager: {{fileID: {SPAWN}}}
  hud: {{fileID: {HUD}}}
  initialSpeed: 12
  maximumSpeed: 28
  speedIncreaseRate: 0.35
  raceDistance: 1000
  applySpeedToPlayer: 0
  onRaceStart:
{unity_event_empty("    ")}  onRacePause:
{unity_event_empty("    ")}  onRaceResume:
{unity_event_empty("    ")}  onRaceFinish:
{unity_event_empty("    ")}{MARKER_END}
"""
    text = text.rstrip() + "\n" + block + "\n"
    SCENE.write_text(text, encoding="utf-8")
    print(f"Patched {SCENE}")


if __name__ == "__main__":
    patch_scene()
