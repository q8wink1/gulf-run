#!/usr/bin/env python3
"""Patch Gameplay.unity with Sprint 23.11 GameRulesManager (config + RaceManager bridge)."""

from __future__ import annotations

from pathlib import Path

ROOT = Path(r"C:\Projects\GulfRun")
SCENE = ROOT / "Client" / "Assets" / "_Project" / "Scenes" / "Gameplay.unity"

GUID_GAME_RULES_MANAGER = "b20c00000000000000000000000000ea"
GUID_CONFIG_ASSET = "b20c00000000000000000000000000eb"
RACE_MANAGER_MB = 540000003

GO, TR, MB = 550000001, 550000002, 550000003
MARKER_START = "# --- SPRINT-23.11-GAME-RULES ---"
MARKER_END = "# --- END SPRINT-23.11-GAME-RULES ---"


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
  m_Name: GameRulesManager
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
  m_Script: {{fileID: 11500000, guid: {GUID_GAME_RULES_MANAGER}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  raceManager: {{fileID: {RACE_MANAGER_MB}}}
  rulesConfig: {{fileID: 11400000, guid: {GUID_CONFIG_ASSET}, type: 2}}
  maximumPlayers: 4
  raceDistance: 1000
  timeLimitSeconds: 0
  eliminationEnabled: 0
  respawnEnabled: 0
  winCondition: 0
  onRaceStarted:
{unity_event_empty("    ")}  onRacePaused:
{unity_event_empty("    ")}  onRaceFinished:
{unity_event_empty("    ")}{MARKER_END}
"""
    text = text.rstrip() + "\n" + block + "\n"
    SCENE.write_text(text, encoding="utf-8")
    print(f"Patched {SCENE}")


if __name__ == "__main__":
    patch_scene()
