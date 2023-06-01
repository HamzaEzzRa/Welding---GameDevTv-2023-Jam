using System;

public static class GameEvents
{
    public static Action<bool> FocusModeEvent;
    public static void FocusModeInvoke(bool isFocused)
    {
        FocusModeEvent?.Invoke(isFocused);
    }

    public static Action<TextButton> TextButtonPointerEnterEvent;
    public static void TextButtonPointerEnterInvoke(TextButton textButton)
    {
        TextButtonPointerEnterEvent?.Invoke(textButton);
    }
}
