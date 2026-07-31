# Live Operations Strategy

**Last updated:** 2026-07-31  
**Owner:** LiveOps Lead + Producer  
**Audience:** LiveOps, Design, Engineering, Support, Marketing

---

## 1. Mission

Operate GulfRun as a **service**: predictable content cadence, safe remote control, measurable economy health, and rapid incident response — indefinitely after Global Launch.

## 2. Operating principles

1. **Config over binary** when store timelines allow.
2. **Staged rollouts** — never 100% on first push for risky changes.
3. **Kill switches** on every major mode, offer, and event.
4. **One calendar** shared across Design, Eng, Marketing, Support.
5. **Economy observability** equal to technical observability.

## 3. Cadence (steady state target)

| Cadence | Example |
|---------|---------|
| Daily | Offers, login cal, light quests |
| Weekly | Weekend events, balance tweaks |
| Seasonal (6–12 weeks) | Battle pass / season content, cosmetics |
| Quarterly | Major mode/meta beats |
| Yearly | Anniversary / franchise moments |

Exact pillars owned by Design; engineering enables pipelines by **M4**.

## 4. LiveOps technical pillars

| Pillar | Capability |
|--------|------------|
| Remote config | Tunables, feature flags, cohorts |
| Events | Schedule, rules, rewards tables |
| Offers / shop | Targeted pricing, segmentation |
| Content | Addressables catalogs / seasons |
| Messaging | Push, in-game inbox |
| Customer tools | Account lookup, grant/revoke (audited) |
| Analytics | Funnels, retention, LTV, cheat signals |

## 5. Environments & promotion

`dev → staging → softlaunch/prod` with schema validation CI.  
Prod pushes require dual control for economy-impacting configs (LiveOps + Tech/Economy approver).

## 6. Segmentation

- Country / storefront / platform
- Player tenure / spend cohort (privacy-compliant)
- Device tier (performance flags)
- Experiment bucketing (A/B) with ethics/review for sensitive changes

## 7. Incident management

### Severity

| Sev | Definition | Response |
|-----|------------|----------|
| Sev-1 | Progress/economy broken widely; major outage; active exploit | Immediate war room |
| Sev-2 | Mode degraded; significant minority impacted | Same-day |
| Sev-3 | Minor; workaround exists | Scheduled |
| Sev-4 | Cosmetic / polish | Backlog |

### Process

Detect → Triage → Mitigate (flag/rollback) → Communicate → Fix → Postmortem (Sev-1/2).

On-call rotations staffed from Soft Launch onward.

## 8. Economy ops

- Dashboards: sources/sinks, inflation indices, offer conversion, refund rates.
- Simulation tools (`Tools/economy-sim/`) run before seasonal launches.
- Compensation policy catalogued (Support macros).
- Freeze windows before major launches.

## 9. Content freeze & cert

- Binary cert freezes scheduled on calendar.
- Remote content may continue if compatible with live binary protocol version.
- Incompatible content blocked by version gates.

## 10. Player support

- Tiered support; CRM integration (see External Services).
- Tooling: inventory view, purchase history, ban status, device list (policy-limited).
- All privileged actions audited.

## 11. Communications

- Status page for outages (public or gated).
- In-game inbox for compensation.
- Store What’s New aligned with release train.

## 12. KPIs (examples)

- D1/D7/D30 retention  
- Crash-free users  
- Revenue / DAU  
- Event participation rate  
- Support tickets / DAU  
- Config rollback count  
- Time to mitigate Sev-1  

## 13. Team model (mature)

- LiveOps producers / managers  
- Economy designer  
- Content engineers  
- On-call service owners  
- Community / support leads  

## 14. Readiness gates

- **M4:** Flag + event + catalog pipeline demo  
- **M5:** Support tools + dashboards + on-call  
- **M8:** Two seasons without process-caused Sev-1  
