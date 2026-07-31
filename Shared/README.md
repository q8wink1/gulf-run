# Shared

Cross-client/server contracts and constants.

**Owner:** Principal Architect

| Path | Contents |
|------|----------|
| `protobuf/` | IDL source (or alternative per ADR) |
| `openapi/` | External HTTP contracts if needed |
| `json-schemas/` | LiveOps config / content manifests |
| `constants/` | Shared enums / reason codes |
| `generated/` | CI-generated stubs — do not hand-edit |

Wire format breaking changes require versioning + ADR when cross-cutting.
