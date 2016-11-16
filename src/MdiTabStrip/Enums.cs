namespace MdiTabStrip
{
    /// <summary>
    /// Specifies the direction in which a scroll event initiated.
    /// </summary>
    public enum ScrollDirection
    {
        None = 0,
        Left = 1,
        Right = 2
    }

    /// <summary>
    /// Specifies the type of tab the <see cref="MdiScrollTab"/> object has been initialized as.
    /// </summary>
    public enum ScrollTabType
    {
        ScrollTabLeft = 1,
        ScrollTabRight = 2,
        ScrollTabDropDown = 3
    }

    /// <summary>
    /// Specifies the desired permanance for the tabs of a <see cref="MdiTabStrip"/>.
    /// </summary>
    public enum MdiTabPermanence
    {
        None = 0,
        First = 1,
        LastOpen = 2
    }

    /// <summary>
    /// Specifies whether or not to obey each form's individual property settings or force each to form
    /// to always be maximized.
    /// </summary>
    public enum MdiChildWindowState
    {
        Normal = 1,
        Maximized = 2
    }

    public enum AnimationType
    {
        None = 0,
        FadeIn = 1,
        FadeOut = 2
    }

    public enum TabType
    {
        Active = 1,
        Inactive = 2,
        MouseOver = 3
    }
}
