namespace Lekser.Enums
{
    public enum CommentState
    {
        OneLineCommit,
        MultiLineCommit,
        CommitBegining,
        CommitEnding,
        CommitEnd,
        NormalText
    }

    public enum LineState
    {
        LastLetterIsDelimeter = 0,
        LineEnded = 1,
        Procesing = 2
    }

    public enum DigitType
    {
        Digit = 0,
        Point = 1,
        E = 2,
        Sign = 3,
        Error = 4
    }

    public enum NumberSystemType
    {
        Binary = 1,
        Octal = 2,
        Hexadecimal = 3,
        Decimal = 4,
        Error = 5
    }

    public enum NumberState
    {
        Digit = 0,
        Point = 1,
        DigitAfterPoint = 2,
        E = 3,
        Sign = 4,
        DigitAfterSign = 5,
        Error = 6,
        FirstSign = 7
    }

    public enum NumberRankType
    {
        Whole = 0,
        FixedPoint = 1,
        FloatPoint = 2,
        Error = 3
    }
}
