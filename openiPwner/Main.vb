Public Class Main

    Dim dlgFirmware1 As New OpenFileDialog, _
           hardware As String, _
           hardware_x As String, _
           KEY As String, _
           IV As String, _
           iBootFold As String

    ' A subroutine that will execute a .EXE and wait til its done.
    Sub DoCMD(ByVal file As String, ByVal arg As String)
        Dim procNlite As New Process
        winstyle = 1
        procNlite.StartInfo.FileName = file
        procNlite.StartInfo.Arguments = " " & arg
        procNlite.StartInfo.WindowStyle = winstyle
        Application.DoEvents()
        procNlite.Start()
        Do Until procNlite.HasExited
            Application.DoEvents()
            For i = 0 To 5000000
                Application.DoEvents()
            Next
        Loop
        procNlite.WaitForExit()
    End Sub

    ' BSPatch, xpwntool encrypt/decrypt subroutines :)
    Sub bspatch(ByVal OldFile, ByVal NewFile, ByVal PatchFile)
        DoCMD("bspatch.exe", " " & Chr(34) & OldFile & Chr(34) & " " & Chr(34) & NewFile & Chr(34) & " " & Chr(34) & PatchFile & Chr(34))
    End Sub
    Sub XpwnToolDecrypt(ByVal InputFile, ByVal OutputFile, ByVal iKey, ByVal iIV)
        DoCMD("xpwntool.exe", " " & Chr(34) & InputFile & Chr(34) & " " & Chr(34) & OutputFile & Chr(34) & " -k " & iKey & " -iv " & iIV)
    End Sub
    Sub XpwnToolEncrypt(ByVal InputFile, ByVal OutputFile, ByVal iTemplate, ByVal iKey, ByVal iIV)
        DoCMD("xpwntool.exe", " " & Chr(34) & InputFile & Chr(34) & " " & Chr(34) & OutputFile & Chr(34) & " " & Chr(34) & " -t " & iTemplate & Chr(34) & " -k " & iKey & " -iv " & iIV)
    End Sub


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        Button1.Enabled = False
        Button1.Text = ("Select iPSW for 3.1.2")

        With dlgFirmware1
            .Title = "Select your 3.1.2 firmware"
            .FileName = ""
            .Filter = "Apple Software Update Files|*.ipsw"
            .ShowDialog()
        End With

        If dlgFirmware1.FileName = "" Then
            Button1.Text = ("Let's Pwn my 3.1.2 iBoot")
            Button1.Enabled = True
            Exit Sub
        End If

        If InStr(dlgFirmware1.FileName, "iPhone2,1") Then
            Label1.Text = "iPhone 3G [S]"
        ElseIf InStr(dlgFirmware1.FileName, "iPod2,1") Then
            Label1.Text = "iPod Touch 2G"
        ElseIf InStr(dlgFirmware1.FileName, "iPod3,1") Then
            Label1.Text = "iPod Touch 3G"
        Else
            MsgBox("Invalid iPSW, Please select either a iPhone 3GS, iPod Touch 2G (MC) or iPod Touch 3G.", MsgBoxStyle.Critical)
            Button1.Text = "Invalid iPSW!" : Exit Sub
        End If

        Button1.Text = ("Preparing...")

        If Label1.Text = "iPod Touch 2G" Then
            hardware_x = "n72"
            hardware = "n72ap"
            KEY = "191b6846543d7026b6f0d5247f030588"
            IV = "5e421f8ce8c811311bbbb8a734ec07ce"
            iBootFold = "iPod2,1"
        End If

        If Label1.Text = "iPod Touch 3G" Then
            hardware_x = "n18"
            hardware = "n18ap"
            KEY = "eabfe2fc4bad7f97c090ed021e7fd80a7463251c4919a3c637d2a0c529b79d5a"
            IV = "6e7dc46c6a9cf573dda9af515c1b50ac"
            iBootFold = "iPod3,1"
        End If

        If Label1.Text = "iPhone 3G [S]" Then
            hardware_x = "n88"
            hardware = "n88ap"
            KEY = "c72ab4aae971f3a9ec356dfe555e4aef72d8e96c480698445ac236904e6a3443"
            IV = "127aa60e77da219961ee70707f44cbd4"
            iBootFold = "iPhone2,1"
        End If


        decrypted_iBoot = Application.StartupPath & "\custom\iBoot." & hardware & ".DECRYPTED.img3"
        patched_iBoot = Application.StartupPath & "\custom\iBoot." & hardware & ".PATCHED.img3"
        pwned_iBoot = Application.StartupPath & "\custom\iBoot.payload"
        extracted_iBoot = Application.StartupPath & "\custom\iBoot." & hardware & ".EXTRACTED.img3"

        For i = 0 To 500000
            Application.DoEvents()
        Next

        System.IO.Directory.CreateDirectory("custom")

        Button1.Text = ("Unzipping...")
        '     Clipboard.SetText("unzip.exe" & " -o " & Chr(34) & dlgFirmware1.FileName.ToString & Chr(34) & " " & Chr(34) & extracted_iBoot & Chr(34))

        DoCMD("unzip.exe", " -o " & Chr(34) & dlgFirmware1.FileName & Chr(34) & " " & Chr(34) & "Firmware\all_flash\all_flash." & hardware & ".production\iBoot." & hardware & ".RELEASE.img3" & Chr(34))
        System.IO.File.Move(Application.StartupPath & "\Firmware\all_flash\all_flash." & hardware & ".production\iBoot." & hardware & ".RELEASE.img3", extracted_iBoot)

        Button1.Text = ("DONE!")
        '
        Button1.Text = ("Decrypting iBoot")
        XpwnToolDecrypt(extracted_iBoot, decrypted_iBoot, KEY, IV)

        Button1.Text = ("Pwning the iBoot..")
        bspatch(decrypted_iBoot, patched_iBoot, "patch" & "\" & iBootFold & "\iBoot.patch")

        Button1.Text = ("Encrypting iBoot")
        XpwnToolEncrypt(patched_iBoot, pwned_iBoot, extracted_iBoot, KEY, IV)

        Button1.Text = ("Moving iBoot to Desktop")
        System.IO.File.Move(pwned_iBoot, Environment.GetFolderPath(0) & "\iBoot.payload")

        Button1.Text = ("Cleaning Up..")
        System.IO.Directory.Delete("Firmware", True)
        System.IO.File.Delete(pwned_iBoot)
        System.IO.File.Delete(extracted_iBoot)
        System.IO.File.Delete(decrypted_iBoot)
        System.IO.File.Delete(patched_iBoot)

        Button1.Text = ("Done Pwning iBoot. (check desktop)")

    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start(LinkLabel1.Text)
    End Sub

End Class
