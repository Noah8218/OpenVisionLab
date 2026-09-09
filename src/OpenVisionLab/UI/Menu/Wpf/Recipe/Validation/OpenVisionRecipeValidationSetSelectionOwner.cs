using System;
using System.Collections.Generic;

namespace OpenVisionLab
{
    // Owns Validation Set option, pinned-split, and image-row selection state.
    internal sealed class OpenVisionRecipeValidationSetSelectionOwner
    {
        private readonly OpenVisionRecipeValidationSetDocumentOwner documentOwner;
        private IReadOnlyList<OpenVisionRecipeValidationSetOption> options =
            Array.Empty<OpenVisionRecipeValidationSetOption>();
        private OpenVisionRecipeValidationSetOption selected;
        private OpenVisionRecipeValidationSetOption train;
        private OpenVisionRecipeValidationSetOption validation;
        private OpenVisionRecipeValidationSetOption test;
        private IReadOnlyList<OpenVisionRecipeValidationSetImageRow> imageRows =
            Array.Empty<OpenVisionRecipeValidationSetImageRow>();
        private OpenVisionRecipeValidationSetImageRow selectedImage;

        internal OpenVisionRecipeValidationSetSelectionOwner(
            OpenVisionRecipeValidationSetDocumentOwner documentOwner)
        {
            this.documentOwner = documentOwner ?? throw new ArgumentNullException(nameof(documentOwner));
        }

        internal IReadOnlyList<OpenVisionRecipeValidationSetOption> Options => options;

        internal OpenVisionRecipeValidationSetOption Selected => selected;

        internal OpenVisionRecipeValidationSetOption Train => train;

        internal OpenVisionRecipeValidationSetOption Validation => validation;

        internal OpenVisionRecipeValidationSetOption Test => test;

        internal IReadOnlyList<OpenVisionRecipeValidationSetImageRow> ImageRows => imageRows;

        internal OpenVisionRecipeValidationSetImageRow SelectedImage => selectedImage;

        internal void Refresh(
            string preferredName,
            string preferredTrainName,
            string preferredValidationName,
            string preferredTestName,
            string previousImagePath)
        {
            OpenVisionRecipeValidationSetOptionSelection selection = documentOwner.BuildSelection(
                preferredName,
                preferredTrainName,
                preferredValidationName,
                preferredTestName);
            options = selection.Options;
            selected = selection.Selected;
            train = selection.Train;
            validation = selection.Validation;
            test = selection.Test;
            RefreshImageRows(previousImagePath);
        }

        internal void Clear()
        {
            options = Array.Empty<OpenVisionRecipeValidationSetOption>();
            selected = null;
            train = null;
            validation = null;
            test = null;
            imageRows = Array.Empty<OpenVisionRecipeValidationSetImageRow>();
            selectedImage = null;
        }

        internal bool SelectSet(OpenVisionRecipeValidationSetOption value)
        {
            if (value == null && options.Count > 0)
            {
                return false;
            }

            if (ReferenceEquals(selected, value))
            {
                return false;
            }

            selected = value;
            return true;
        }

        internal bool SelectImage(OpenVisionRecipeValidationSetImageRow value)
        {
            if (value == null && imageRows.Count > 0)
            {
                return false;
            }

            if (ReferenceEquals(selectedImage, value))
            {
                return false;
            }

            selectedImage = value;
            return true;
        }

        internal bool SelectTrain(OpenVisionRecipeValidationSetOption value)
        {
            if (value == null && options.Count > 0)
            {
                return false;
            }

            if (ReferenceEquals(train, value))
            {
                return false;
            }

            train = value;
            return true;
        }

        internal bool SelectValidation(OpenVisionRecipeValidationSetOption value)
        {
            if (value == null && options.Count > 0)
            {
                return false;
            }

            if (ReferenceEquals(validation, value))
            {
                return false;
            }

            validation = value;
            return true;
        }

        internal bool SelectTest(OpenVisionRecipeValidationSetOption value)
        {
            if (value == null && options.Count > 0)
            {
                return false;
            }

            if (ReferenceEquals(test, value))
            {
                return false;
            }

            test = value;
            return true;
        }

        internal void ClearPinnedSelections()
        {
            train = null;
            validation = null;
            test = null;
        }

        internal void RefreshImageRows(string previousImagePath)
        {
            OpenVisionRecipeValidationSetImageSelection selection = documentOwner.BuildImageSelection(
                selected,
                previousImagePath);
            imageRows = selection.Rows;
            selectedImage = selection.Selected;
        }
    }
}
