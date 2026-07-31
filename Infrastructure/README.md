# Infrastructure

Infrastructure as Code, environments, networking, and observability-as-code.

**Owner:** DevOps Lead

| Path | Purpose |
|------|---------|
| `terraform/` | Primary IaC (tool locked at M1 ADR) |
| `environments/*` | Per-env overlays / tfvars examples |
| `observability/` | Dashboards, alert rules as code |
| `network/` | Edge, DNS, WAF definitions |

Production applies require manual approval. See [CI/CD](../docs/04-engineering/CI_CD.md).
