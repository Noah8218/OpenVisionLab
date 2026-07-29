# CVR-12 Bounded Matcher Deformation Trigger Audit

Updated: 2026-07-28 KST

## Outcome

Status: Complete for the activation audit.

Activation decision: not admitted.

The current repository does not contain a labelled inspection task proving
that one physical feature fails the frozen matcher because of bounded
deformation. No deformation algorithm, XML parameter, PropertyGrid control,
sample generator, or synthetic success matrix was added.

## Evidence Reviewed

### Commercial reference

The retained transcript for HALCON Shape Matching Advanced Parameters describes
slightly deformed paper clips and a pixel deformation allowance. It also
separately discusses blur tolerance, reduced search domains, polarity, and
scale. This explains a commercial capability but supplies no OpenVisionLab
task, source images, labels, deformation measurement, or acceptance limit.

Source:
`artifacts/commercial_rulebase_video_review_20260727/13_whisper_base_en.txt`.

### Current OpenVisionLab matching evidence

- `Public_Edge_Fiducial` contains one synthetic Good image and one
  different/wrong-feature NG image. It is target-identity evidence, not a
  deformation series.
- `Public_Matching_DiePad` contains target and no-target scenes. It does not
  label shape deformation.
- P220/P225 card evidence concerns wrong glyph identity, search ROI, pose, and
  score/uniqueness behavior.
- P226-P235 Auto MPoint evidence concerns candidate selection, locator
  stability, and product N-image workflow.
- CVR-11 uses a project-authored globally inverted copy of one rigid feature.
  It explicitly does not prove deformation.
- No current public filename, manifest row, report, or retained matcher matrix
  identifies elastic, non-rigid, warped, bent, or landmark-displacement truth.

The audit therefore cannot separate deformation from a different target,
uniform or anisotropic scale, pose, blur, polarity, occlusion, or an incorrect
search ROI.

## Required Admission Packet

CVR-12 may be reconsidered only when one packet contains all of the following:

1. **Named operator task**
   - part/feature identity;
   - why the feature is expected to deform physically;
   - whether deformed instances should be accepted, rejected, or measured.
2. **Frozen coordinate contract**
   - one approved template;
   - one fixed search ROI or an already-qualified fixture;
   - allowed rotation and uniform-scale ranges;
   - fixed polarity and preprocessing contract.
3. **Numeric deformation truth**
   - reviewed landmarks, contour correspondence, or another reproducible
     displacement measurement;
   - unit (`px` initially unless calibrated evidence exists);
   - documented allowed maximum and reject boundary;
   - direction/region in which deformation is permitted.
4. **Nuisance exclusion**
   - pose and uniform-scale normalization residual;
   - anisotropic X/Y scale check, which belongs to CVR-13 when causal;
   - blur/focus measure;
   - illumination/polarity state;
   - occlusion/crop and ROI containment review.
5. **Frozen baseline failure**
   - unchanged current EdgeBasedMatching XML;
   - Same/unique/polarity/angle/scale/search settings and DLL hash;
   - drawings showing the intended physical feature;
   - failures correlated with the deformation label rather than a nuisance.
6. **Data split**
   - Train for the numeric limit;
   - Validation for one frozen candidate;
   - untouched Held-out replay;
   - exact image/label hashes and no duplicate leakage.

Recommended minimum before implementation is 12 accepted deformed targets and
12 rejected/out-of-limit or no-target cases across Train/Validation/Held-out.
This count is an admission minimum, not a production-qualification claim.

## First Implementation Gate After Admission

Even with a valid packet, implementation must begin with a static design and
baseline replay. The first candidate must:

- have one numeric deformation limit;
- remain opt-in with missing-key legacy behavior;
- retain current score, uniqueness, ROI, angle, scale, polarity, and
  result-count gates;
- publish deformation evidence and an exact reject reason;
- provide PropertyGrid/XML/report round trip;
- replay the untouched Held-out split;
- avoid PatFlex, elastic-matching, or commercial-parity claims.

Do not choose control-point warping, chamfer-distance relaxation, local search,
or another algorithm until the packet shows which deformation model is
actually required.

## Reopen Command

```text
Audit the supplied CVR-12 packet against
docs/reports/OPENVISIONLAB_CVR12_TRIGGER_AUDIT_20260728.md.
Do not implement until all six admission sections pass. Preserve the frozen
baseline failure and held-out split before evaluating one bounded design.
```

## Completion Record

```text
Status: Complete
Scope: Read-only CVR-12 activation audit and reusable admission packet; no matcher implementation or generated deformation data.
Acceptance criteria: Commercial reference separated from project evidence; current matching assets checked; deformation/nuisance evidence gap named; numeric packet and held-out gate recorded.
Verification: Repository/document/artifact text inventory; HALCON retained transcript review; public matching sample and manifest inventory; current CVR-11 contract comparison.
Evidence: docs/reports/OPENVISIONLAB_CVR12_TRIGGER_AUDIT_20260728.md
Boundary / next dependency: A named physical feature and complete six-section labelled packet are required before CVR-12 implementation.
```
