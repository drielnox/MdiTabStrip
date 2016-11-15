Imports System.Drawing.Drawing2D

Friend Class MdiMenuStripRenderer
    Inherits ToolStripRenderer

    Protected Overrides Sub OnRenderToolStripBorder(ByVal e As ToolStripRenderEventArgs)
        MyBase.OnRenderToolStripBorder(e)

        ControlPaint.DrawFocusRectangle(e.Graphics, e.AffectedBounds, SystemColors.ControlDarkDark, SystemColors.ControlDarkDark)
    End Sub

    Protected Overrides Sub OnRenderToolStripBackground(ByVal e As ToolStripRenderEventArgs)
        MyBase.OnRenderToolStripBackground(e)

        Dim strip As ToolStrip = e.ToolStrip
        Dim h As Double = strip.Height / strip.Items.Count

        Using scratchImage As New Bitmap(strip.Width, strip.Height)
            Using g As Graphics = Graphics.FromImage(scratchImage)
                Dim rect As New RectangleF(0, 0, scratchImage.Width, h)

                g.FillRectangle(Brushes.White, New Rectangle(New Point(0, 0), scratchImage.Size))

                For Each item As MdiMenuItem In strip.Items
                    If item.IsTabVisible Then
                        g.FillRectangle(Brushes.White, rect)
                    Else
                        g.FillRectangle(New SolidBrush(Color.FromArgb(255, 225, 225, 225)), rect)
                    End If
                    rect.Offset(0, h)
                Next
            End Using

            e.Graphics.DrawImage(scratchImage, e.AffectedBounds)
        End Using
    End Sub

    Protected Overrides Sub OnRenderItemText(ByVal e As System.Windows.Forms.ToolStripItemTextRenderEventArgs)
        Dim mdiItem As MdiMenuItem = e.Item

        If mdiItem.IsMouseOver Then
            If mdiItem.IsTabActive Then
                e.TextColor = Color.Black
            Else
                e.TextColor = Color.White
            End If
        End If

        If mdiItem.IsTabActive Then
            e.TextFont = New Font(e.TextFont, FontStyle.Bold)
        End If

        MyBase.OnRenderItemText(e)
    End Sub

    Protected Overrides Sub OnRendermenuItemBackground(ByVal e As System.Windows.Forms.ToolStripItemRenderEventArgs)
        MyBase.OnRenderItemBackground(e)

        Dim mdiItem As MdiMenuItem = e.Item

        If mdiItem.IsMouseOver Then
            If mdiItem.IsTabActive Then
                e.Graphics.DrawRectangle(Pens.Black, e.Item.ContentRectangle)
            Else
                e.Graphics.FillRectangle(Brushes.Black, e.Item.ContentRectangle)
            End If
        End If
    End Sub

    Protected Overrides Sub OnRenderItemImage(ByVal e As System.Windows.Forms.ToolStripItemImageRenderEventArgs)
        Dim mdiItem As MdiMenuItem = e.Item

        If Not mdiItem.IsTabActive Then
            MyBase.OnRenderItemImage(e)
        End If
    End Sub

    Protected Overrides Sub OnRenderItemCheck(ByVal e As System.Windows.Forms.ToolStripItemImageRenderEventArgs)
        Dim mdiItem As MdiMenuItem = e.Item
        Dim tsi As New ToolStripItemImageRenderEventArgs(e.Graphics, e.Item, mdiItem.CheckedImage, e.ImageRectangle)
        MyBase.OnRenderItemImage(tsi)
    End Sub
End Class
