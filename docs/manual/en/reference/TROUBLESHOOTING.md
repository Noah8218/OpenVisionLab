# Troubleshooting order

## The result does not change

1. Confirm that you selected Preview or Run Review.
2. Confirm the current input Layer.
3. Confirm that the previous Step created its output Layer.
4. Check whether you changed only a display option.

## ResultCount is zero

Check the input image and ROI, threshold polarity/range, minimum Area/Score/
Contrast, Template or dependency file, then Search ROI and pose ranges.

## Too many results

Narrow the ROI, find noise or repeated patterns in drawings, adjust Area/Width/
Height or Score, and rerun both Good and Bad.

## Pipeline is NG

Select the first NG Step, open its input Layer image, check its route, ROI, and
main parameters, then select Run Review again.

## Guide does not open

The distribution `Guide` folder beside `OpenVisionLab.exe` must contain:

```text
Guide/OpenVisionLab_User_Manual.ko.html
Guide/OpenVisionLab_User_Manual.en.html
Guide/guide-manifest.json
```

If the selected-language file is missing or damaged, restore the complete
`Guide` folder or reinstall the application. Do not replace it with files copied
from the repository `docs` folder.
