Public Class Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RichTextBox1.LoadFile(Application.StartupPath + "\license.rtf")
        Catch ex As Exception
            RichTextBox1.Text = "{license file not found}"
        End Try
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub
End Class