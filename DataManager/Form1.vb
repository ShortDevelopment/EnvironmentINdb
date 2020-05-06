Imports GMap.NET.WindowsForms
Imports System.Net
Imports System.Threading
Imports System.ComponentModel

Public Class Form1
    Public Shared Property Data As String
    Property Markers As GMapOverlay = New GMapOverlay("markers")
    Sub RunOnNewThread(deleg As ParameterizedThreadStart)
        Dim a As New Thread(deleg)
        a.IsBackground = True
        a.Start()
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MenuStrip1.ForeColor = Color.Orange
        GMapControl1.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance
        GMapControl1.Position = New GMap.NET.PointLatLng(48.9016232, 9.0812541)
        GMapControl1.Zoom = 11
        GMapControl1.ShowCenter = False
        GMapControl1.Overlays.Add(Markers)
    End Sub
    Sub HandleData()
        Markers.Clear()
        wait.Show()
        RunOnNewThread(Sub()
                           Try
                               Dim a = Split(TextBox1.Text, "<br/>")
                               Dim mydata As New List(Of Marker)
                               For Each b In a
                                   If (b.Contains("<")) Then Continue For
                                   If b = "" Or b.Contains(vbNewLine) Then Continue For
                                   Dim c = Split(b.Replace(".", ","), "|")
                                   Try
                                       'System.Diagnostics.Debug.Print("---[\"" + (double.Parse(c[0])).ToString() + "\"]---");
                                       'Toast.MakeText(this, c[0] + "|" + c[1] + "|" + c[2] + "|", ToastLength.Short).Show();
                                       Dim KommStr = ""
                                       If Not c(4) = "" Then
                                           KommStr = vbNewLine + c(4)
                                       End If
                                       mydata.Add(New Marker(Int(c(2)), Double.Parse(c(1)), Double.Parse(c(0)), c(3) + KommStr))
                                   Catch : End Try
                               Next
                               Dim newdata = mydata.Where(Function(x)
                                                              Dim abc As Boolean = True
                                                              Dim t1 = Me.Invoke(Function() TextBox3.Text)
                                                              If Not t1 = Nothing Then
                                                                  Dim min = Split(t1, "|")(0)
                                                                  Dim max = Split(t1, "|")(1)
                                                                  If x.db >= min AndAlso x.db <= max Then
                                                                      'abc = True
                                                                  Else
                                                                      abc = False
                                                                  End If
                                                              End If
                                                              Dim t2 = Me.Invoke(Function() TextBox2.Text)
                                                              If Not t2 = Nothing Then
                                                                  Dim _hour1 = Int(Split(Split(t2, "-")(0), ":")(0))
                                                                  Dim _min1 = Int(Split(Split(t2, "-")(0), ":")(1)) + _hour1 * 60
                                                                  Dim _hour2 = Int(Split(Split(t2, "-")(1), ":")(0))
                                                                  Dim _min2 = Int(Split(Split(t2, "-")(1), ":")(1)) + _hour2 * 60
                                                                  Dim ac = Split(Split(x.info, ":")(0), " ")
                                                                  Dim hour = Int(ac(ac.Length - 1))
                                                                  Dim min = Int(Split(Split(x.info, ":")(1), " ")(0)) + hour * 60
                                                                  If min < _min2 AndAlso min > _min1 Then

                                                                  Else
                                                                      abc = False
                                                                  End If
                                                              End If
                                                              If Not Me.Invoke(Function() ComboBox1.Text) = "Alle" Then
                                                                  If x.info.Contains(Me.Invoke(Function() ComboBox1.Text)) Then
                                                                      'abc = True
                                                                  Else
                                                                      abc = False
                                                                  End If
                                                              End If
                                                              Return abc
                                                          End Function)
                               For Each d As Marker In newdata
                                   Me.Invoke(Sub() AddMarker(d))
                               Next
                               Me.Invoke(Sub()
                                             wait.Hide()
                                             Label4.Text = "Treffer: " + newdata.Count.ToString()
                                         End Sub)
                           Catch ex As Exception
                               MsgBox("Ein Fehler ist aufgetreten!", MsgBoxStyle.Critical)
                           End Try
                       End Sub)
    End Sub
    Structure Marker
        Dim db As Integer
        Dim lat As Double
        Dim lon As Double
        Dim info As String
        Sub New(_db As Integer, _lat As Double, _lon As Double, Optional _info As String = "Keine Infos")
            db = _db
            lat = _lat
            lon = _lon
            info = _info
        End Sub
    End Structure
    Sub AddMarker(m As Marker)
        Dim marker As GMapMarker = New Markers.GMarkerGoogle(New GMap.NET.PointLatLng(m.lat, m.lon), CreatePuspin(GetColor(m.db)))
        marker.ToolTipText = m.db.ToString + "db" + vbNewLine + m.info
        Markers.Markers.Add(marker)
    End Sub
    Private Function GetColor(val As Integer) As Color
        Dim r As Double = 0, g As Double = 255
        Dim a As Double = Math.Pow(1.071, val + 7) 'val * 3.5
        If (a > 255) Then
            r = 255
            g = 255 + (255 - a)
        Else
            r = a
        End If
        If g < 0 Then g = 0
        Return Color.FromArgb(255, r, g, 0)
    End Function
    Function CreatePuspin(col As Color) As Bitmap
        Dim rect = New Rectangle(1, 1, 11, 11)
        Dim a As New Bitmap(12, 12)
        Dim g As Graphics = Graphics.FromImage(a)
        g.FillEllipse(New SolidBrush(col), rect)
        If Me.Invoke(Function() CheckBox1.Checked) Then g.DrawEllipse(New Pen(Color.Black), New Rectangle(0, 0, 12, 12))
        Return a
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ComboBox1.Text = "Alle"
        TextBox2.Text = Nothing
        TextBox3.Text = Nothing
        CheckBox1.Checked = True
        HandleData()
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        HandleData()
    End Sub
    Private Sub LoadDataFrimFileToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim a As New OpenFileDialog
        a.Filter = "Text-dateien|*.txt"
        If a.ShowDialog = DialogResult.OK Then
            Data = My.Computer.FileSystem.ReadAllText(a.FileName)
            wait.Show()
            RunOnNewThread(Sub()
                               TextBox1.Text = Data
                               HandleData()
                               Me.Invoke(Sub() wait.Hide())
                           End Sub)
        End If
    End Sub
    Private Sub GMapControl1_MouseWheel(sender As Object, e As MouseEventArgs) Handles GMapControl1.MouseWheel
        Label5.Text = "Zoom: " + sender.Zoom.ToString()
    End Sub
    Private Sub GMapControl1_MouseMove(sender As Object, e As MouseEventArgs) Handles GMapControl1.MouseMove
        Label6.Text = "Location: [" + GMapControl1.Position.ToString() + "]"
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        GMapControl1.Position = New GMap.NET.PointLatLng(48.9016232, 9.0812541)
        GMapControl1.Zoom = 19
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        GMapControl1.Position = New GMap.NET.PointLatLng(48.9016232, 9.0812541)
        GMapControl1.Zoom = 11
    End Sub
    Private Sub LoaddataToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles LoaddataToolStripMenuItem.Click
        Dim a As New WebClient
        wait.Show()
        a.Credentials = New NetworkCredential("jonjon0815", "Toaster1144")
        RunOnNewThread(Sub()
                           Data = a.DownloadString("https://umwelt.wasweisich.com/get/")
                           Me.Invoke(Sub()
                                         wait.Hide()
                                         TextBox1.Text = Data
                                         HandleData()
                                     End Sub)
                       End Sub)
    End Sub
    Private Sub MicrosoftToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MicrosoftToolStripMenuItem.Click
        GMapControl1.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance
    End Sub
    Private Sub GoogleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GoogleToolStripMenuItem.Click
        GMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance
    End Sub
    Private Sub ScreenshotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ScreenshotToolStripMenuItem.Click
        Dim bmp As New Bitmap(GMapControl1.Width, GMapControl1.Height)
        GMapControl1.DrawToBitmap(bmp, New Rectangle(0, 0, GMapControl1.Width, GMapControl1.Height))
        Dim a As New SaveFileDialog
        a.Filter = "JPEG-Datei|*.jpg|PNG-Datei|*.png"
        If a.ShowDialog = DialogResult.OK Then
            bmp.Save(a.FileName)
        End If
    End Sub
    Private Sub ExtrasToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExtrasToolStripMenuItem.Click
        Form2.Show()
        Form2.BringToFront()
    End Sub
End Class
Public Class LKTextBox2
    Inherits TextBox
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
    Dim _hint As String
    Property Hint As String
        Get
            Return _hint
        End Get
        Set(value As String)
            _hint = value
            Me.Invalidate()
            Me.Update()
            Me.Refresh()
            Application.DoEvents()
        End Set
    End Property
    Private Const WM_PAINT As Int32 = &HF
    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        MyBase.WndProc(m)
        If m.Msg = WM_PAINT AndAlso Me.TextLength = 0 Then
            Using g = Me.CreateGraphics
                g.DrawString(Hint, Me.Font, Brushes.Gray, 1, 1)
            End Using
        End If
    End Sub
End Class