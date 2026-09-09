using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class LearnDocumentationCopyPolicy
{
    private static readonly string[] ForbiddenPhrases =
    {
        "must not",
        "does not run Preview",
        "no Preview/Run",
        "no layer change",
        "실행하지 않습니다",
        "바뀌면 안",
        "Tool View만",
        "도구만 열",
        "명시 Preview/Run",
        "명시적 Preview",
        "명시적으로 실행",
        "명시 액션",
        "자동 실행된다고 가정",
        "덮어쓰지 않습니다",
        "explicit Preview/Run",
        "execution evidence",
        "smoke evidence",
        "runtime contract",
        "public sample contract",
        "implementation order",
        "tool gap backlog",
        "설치나 카메라"
    };

    internal static string? FindForbiddenPhrase(string content)
    {
        ArgumentNullException.ThrowIfNull(content);
        return ForbiddenPhrases.FirstOrDefault(
            phrase => content.Contains(phrase, StringComparison.OrdinalIgnoreCase));
    }

    internal static void AssertDocumentHasNoInternalContractCopy(string documentPath, string context)
    {
        string content = File.ReadAllText(documentPath);
        string? forbidden = FindForbiddenPhrase(content);
        if (!string.IsNullOrWhiteSpace(forbidden))
        {
            throw new InvalidOperationException(
                "OpenVision Learn document contains internal engineering copy '"
                + forbidden
                + "'. Document="
                + context);
        }
    }

    internal static void AssertVisibleCopyHasNoInternalContractCopy(
        IEnumerable<string> copy,
        string context)
    {
        ArgumentNullException.ThrowIfNull(copy);
        string allCopy = string.Join(
            " | ",
            copy.Where(item => !string.IsNullOrWhiteSpace(item)));
        string? forbidden = FindForbiddenPhrase(allCopy);
        if (!string.IsNullOrWhiteSpace(forbidden))
        {
            throw new InvalidOperationException(
                context + " contains internal engineering copy '" + forbidden + "'.");
        }
    }
}
