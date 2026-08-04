#!/usr/bin/env python3
"""Generate Sprint 23.7 SpawnProfile assets + patch Gameplay.unity."""

from __future__ import annotations

from pathlib import Path

ROOT = Path(r"C:\Projects\GulfRun")
SETTINGS = ROOT / "Client" / "Assets" / "_Project" / "Settings" / "SpawnProfiles"
SCENE = ROOT / "Client" / "Assets" / "_Project" / "Scenes" / "Gameplay.unity"

GUID_PROFILE_SCRIPT = "b20c00000000000000000000000000c9"
GUID_SPAWN_MANAGER = "b20c00000000000000000000000000cb"
GUID_FOLDER = "b20c00000000000000000000000000cc"

# Map profile asset guids
PROFILES = {
    "Kuwait": ("b20c00000000000000000000000000cd", 0.55, 0.70, 8.0, 22.0),
    "Dubai": ("b20c00000000000000000000000000ce", 0.60, 0.75, 7.0, 20.0),
    "Doha": ("b20c00000000000000000000000000cf", 0.50, 0.65, 9.0, 26.0),
    "Muscat": ("b20c00000000000000000000000000d0", 0.45, 0.60, 10.0, 28.0),
}

# category enum ordinals: Obstacle=0 Coin=1 PowerUp=2 Decoration=3 ItemBox=4 Npc=5 Gem=6
# Per-group overrides relative to map obstacle baseline (prob, dens, min, max)
GROUP_TEMPLATES = [
    # category, enabled, prob_mul, dens_mul, min_mul, max_mul  (applied to map baseline for Obstacle;
    # other categories use fixed defaults below)
    (0, 1, None),  # Obstacle — uses map baseline
    (1, 1, (0.75, 0.85, 3.0, 12.0)),  # Coin
    (6, 1, (0.20, 0.35, 18.0, 48.0)),  # Gem
    (2, 1, (0.25, 0.40, 20.0, 55.0)),  # PowerUp
    (3, 1, (0.60, 0.70, 4.0, 16.0)),  # Decoration
    (5, 0, (0.15, 0.25, 30.0, 80.0)),  # Npc (disabled placeholders)
]


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


def group_yaml(category: int, enabled: int, prob: float, dens: float, mn: float, mx: float) -> str:
    return f"""  - category: {category}
    enabled: {enabled}
    spawnProbability: {prob}
    spawnDensity: {dens}
    minimumSpacing: {mn}
    maximumSpacing: {mx}
"""


def write_profile(name: str, guid: str, obs_prob: float, obs_dens: float, obs_min: float, obs_max: float) -> None:
    groups = []
    for category, enabled, fixed in GROUP_TEMPLATES:
        if fixed is None:
            prob, dens, mn, mx = obs_prob, obs_dens, obs_min, obs_max
        else:
            prob, dens, mn, mx = fixed
        groups.append(group_yaml(category, enabled, prob, dens, mn, mx))

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
  m_Script: {{fileID: 11500000, guid: {GUID_PROFILE_SCRIPT}, type: 3}}
  m_Name: SpawnProfile_{name}
  m_EditorClassIdentifier: 
  profileId: {name}
  groups:
{''.join(groups)}"""
    path = SETTINGS / f"SpawnProfile_{name}.asset"
    path.write_text(body, encoding="utf-8")
    path.with_suffix(".asset.meta").write_text(meta_asset(guid), encoding="utf-8")
    print(f"Wrote {path}")


def patch_scene() -> None:
    text = SCENE.read_text(encoding="utf-8")
    marker_start = "# --- SPRINT-23.7-SPAWN-MANAGER ---"
    marker_end = "# --- END SPRINT-23.7-SPAWN-MANAGER ---"
    if marker_start in text:
        start = text.find(marker_start)
        end = text.find(marker_end)
        if start >= 0 and end > start:
            end = text.find("\n", end) + 1
            text = text[:start] + text[end:]

    kuwait_guid = PROFILES["Kuwait"][0]
    go, tr, mb = 530000001, 530000002, 530000003
    block = f"""
{marker_start}
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
  m_Name: GameplaySpawnManager
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
  m_Script: {{fileID: 11500000, guid: {GUID_SPAWN_MANAGER}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  spawnProfile: {{fileID: 11400000, guid: {kuwait_guid}, type: 2}}
  trackGenerator: {{fileID: 520000003}}
  randomSeed: 0
  logPlans: 0
{marker_end}
"""
    text = text.rstrip() + "\n" + block + "\n"
    SCENE.write_text(text, encoding="utf-8")
    print(f"Patched {SCENE}")


def main() -> None:
    SETTINGS.mkdir(parents=True, exist_ok=True)
    (Path(str(SETTINGS) + ".meta")).write_text(meta_folder(GUID_FOLDER), encoding="utf-8")
    for name, (guid, prob, dens, mn, mx) in PROFILES.items():
        write_profile(name, guid, prob, dens, mn, mx)
    patch_scene()


if __name__ == "__main__":
    main()
