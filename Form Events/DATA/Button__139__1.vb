'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports B1WizardBase
Imports SAPbobsCOM
Imports SAPbouiCOM

Namespace Factura_Electronica
    
    Public Class Button__139__1
        Inherits B1Item
        
        Public Sub New()
            MyBase.New
            FormType = "133"
            ItemUID = "1"
        End Sub

        '<B1Listener(BoEventTypes.et_CLICK, False)>
        'Public Overridable Sub OnAfterClick(ByVal pVal As ItemEvent)
        '    Dim ActionSuccess As Boolean = pVal.ActionSuccess
        '    Dim form As Form = B1Connections.theAppl.Forms.Item(pVal.FormUID)
        '    Dim item As Item = form.Items.Item("1")
        '    Dim button As Button = CType(item.Specific, Button)
        '    'ADD YOUR ACTION CODE HERE ...

        '    'ADD YOUR ACTION CODE HERE ...

        '    Dim nSum As Decimal = 0.0
        '    Dim nTotal1 As Decimal = 0.0
        '    Dim nTotal2 As Decimal = 0.0
        '    Dim nTotalString As String = ""
        '    Dim nTotalresultado As String = ""


        '    Dim oMatrix As SAPbouiCOM.Matrix
        '    Dim oCheck As SAPbouiCOM.CheckBox

        '    oSerie = form.Items.Item("88").Specific.value.ToString().Trim()

        '    If form.Items.Item("3").Specific.value = "S" Then
        '        oMatrix = form.Items.Item("39").Specific
        '    Else
        '        oMatrix = form.Items.Item("38").Specific
        '    End If



        '    oRecordSetCancelacion = Nothing
        '    oRecordSetCancelacion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
        '    sQuery = "select T0.""IsForCncl"" from NNM1 T0 Where T0.""ObjectCode"" = 13 and T0.""Series"" = " & oSerie
        '    oRecordSetCancelacion.DoQuery(sQuery)
        '    oRecordSetCancelacion.MoveFirst()



        '    If oRecordSetCancelacion.Fields.Item(0).Value = "N" Then


        '        If form.Mode = BoFormMode.fm_ADD_MODE Then


        '            ' If form.Items.Item("63").Enabled = True Then



        '            For iContTotal As Integer = 1 To oMatrix.RowCount - 1


        '                If form.Items.Item("63").Specific.value = "USD" Then

        '                    If form.Items.Item("3").Specific.value = "S" Then


        '                        If Not String.IsNullOrEmpty(oMatrix.Columns.Item("11").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
        '                            nTotalString = oMatrix.Columns.Item("11").Cells.Item(iContTotal).Specific.value.ToString()
        '                            nTotalresultado = nTotalString.Substring(0, 3)
        '                            nSum += oMatrix.Columns.Item("11").Cells.Item(iContTotal).Specific.value.ToString().Replace(nTotalresultado, "").Replace("USD", "")
        '                        End If


        '                    Else

        '                        If Not String.IsNullOrEmpty(oMatrix.Columns.Item("284").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then

        '                            nTotalString = oMatrix.Columns.Item("284").Cells.Item(iContTotal).Specific.value.ToString()
        '                            nTotalresultado = nTotalString.Substring(0, 3)
        '                            nSum += oMatrix.Columns.Item("284").Cells.Item(iContTotal).Specific.value.ToString().Replace(nTotalresultado, "").Replace("USD", "")
        '                        End If


        '                    End If


        '                Else

        '                    If form.Items.Item("3").Specific.value = "S" Then

        '                        If Not String.IsNullOrEmpty(oMatrix.Columns.Item("187").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
        '                            nTotalString = oMatrix.Columns.Item("187").Cells.Item(iContTotal).Specific.value
        '                            nTotalresultado = nTotalString.Substring(0, 3)
        '                            nSum += oMatrix.Columns.Item("187").Cells.Item(iContTotal).Specific.value.Replace(nTotalresultado, "").Replace("USD", "").Replace(",", "")

        '                        End If

        '                    Else

        '                        If Not String.IsNullOrEmpty(oMatrix.Columns.Item("288").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then

        '                            nTotalString = oMatrix.Columns.Item("288").Cells.Item(iContTotal).Specific.value.ToString()
        '                            nTotalresultado = nTotalString.Substring(0, 3)
        '                            nSum += oMatrix.Columns.Item("288").Cells.Item(iContTotal).Specific.value.ToString().Replace(nTotalresultado, "").Replace("USD", "").Replace(",", "")

        '                        End If

        '                    End If


        '                End If





        '            Next

        '            nTotalString = form.Items.Item("29").Specific.value.ToString()
        '            nTotalresultado = nTotalString.Substring(0, 3)
        '            nTotal1 = nSum - form.Items.Item("29").Specific.value.ToString().Replace(nTotalresultado, "").Replace("USD", "").Replace(",", "")


        '            If Not nTotal1 = 0 Then

        '                'form.Freeze(True)

        '                oCheck = form.Items.Item("105").Specific

        '                oCheck.Checked = True

        '                form.Items.Item("103").Specific.value = nTotal1

        '                'form.Items.Item("1").Click()

        '                'form.Freeze(False)
        '            End If

        '            'End If

        '        End If
        '    End If

        '    'oRecordSetCancelacion = Nothing
        '    'oRecordSetCancelacion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
        '    'sQuery = "select T0.""IsForCncl"" from NNM1 T0 Where T0.""ObjectCode"" = 13 and T0.""Series"" = " & oSerie
        '    'oRecordSetCancelacion.DoQuery(sQuery)
        '    'oRecordSetCancelacion.MoveFirst()


        '    'If oRecordSetCancelacion.Fields.Item(0).Value = "N" Then


        '    '    If form.Items.Item("3").Specific.value = "S" Then
        '    '        oMatrix = form.Items.Item("39").Specific
        '    '    Else
        '    '        oMatrix = form.Items.Item("38").Specific
        '    '    End If



        '    '    If form.Items.Item("63").Enabled = True Then


        '    '        For iContTotal As Integer = 1 To oMatrix.RowCount - 1


        '    '            If form.Items.Item("63").Specific.value = "USD" Then

        '    '                If form.Items.Item("3").Specific.value = "S" Then


        '    '                    If Not String.IsNullOrEmpty(oMatrix.Columns.Item("11").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
        '    '                        nSum += oMatrix.Columns.Item("11").Cells.Item(iContTotal).Specific.value.ToString().Replace("QTZ", "").Replace("USD", "")
        '    '                    End If


        '    '                Else

        '    '                    If Not String.IsNullOrEmpty(oMatrix.Columns.Item("284").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
        '    '                        nSum += oMatrix.Columns.Item("284").Cells.Item(iContTotal).Specific.value.ToString().Replace("QTZ", "").Replace("USD", "")
        '    '                    End If


        '    '                End If


        '    '            Else

        '    '                If form.Items.Item("3").Specific.value = "S" Then

        '    '                    If Not String.IsNullOrEmpty(oMatrix.Columns.Item("187").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
        '    '                        nSum += oMatrix.Columns.Item("187").Cells.Item(iContTotal).Specific.value.ToString().Replace("QTZ", "").Replace("USD", "")
        '    '                    End If

        '    '                Else

        '    '                    If Not String.IsNullOrEmpty(oMatrix.Columns.Item("288").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
        '    '                        nSum += oMatrix.Columns.Item("288").Cells.Item(iContTotal).Specific.value.ToString().Replace("QTZ", "").Replace("USD", "")
        '    '                    End If

        '    '                End If


        '    '            End If





        '    '        Next


        '    '        nTotal1 = nSum - form.Items.Item("29").Specific.value.ToString().Replace("QTZ", "").Replace("USD", "")


        '    '        If Not nTotal1 = 0 Then

        '    '            form.Freeze(True)

        '    '            oCheck = form.Items.Item("105").Specific

        '    '            oCheck.Checked = True

        '    '            form.Items.Item("103").Specific.value = nTotal1

        '    '            form.Items.Item("1").Click()

        '    '            form.Freeze(False)
        '    '        End If




        '    '        ' Else

        '    '        '    For iContTotal As Integer = 1 To oMatrix.RowCount - 1


        '    '        '        If form.Items.Item("70").Specific.value = "S" Then


        '    '        '            If form.Items.Item("3").Specific.value = "S" Then

        '    '        '                If Not String.IsNullOrEmpty(oMatrix.Columns.Item("187").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
        '    '        '                    nSum += oMatrix.Columns.Item("185").Cells.Item(iContTotal).Specific.value.ToString().Replace("QTZ", "").Replace("USD", "")
        '    '        '                End If

        '    '        '            Else

        '    '        '                If Not String.IsNullOrEmpty(oMatrix.Columns.Item("38").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
        '    '        '                    nSum += oMatrix.Columns.Item("286").Cells.Item(iContTotal).Specific.value.ToString().Replace("QTZ", "").Replace("USD", "")
        '    '        '                End If

        '    '        '            End If


        '    '        '        Else


        '    '        '            If form.Items.Item("3").Specific.value = "S" Then

        '    '        '                If Not String.IsNullOrEmpty(oMatrix.Columns.Item("187").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
        '    '        '                    nSum += oMatrix.Columns.Item("187").Cells.Item(iContTotal).Specific.value.ToString().Replace("QTZ", "").Replace("USD", "")
        '    '        '                End If

        '    '        '            Else

        '    '        '                If Not String.IsNullOrEmpty(oMatrix.Columns.Item("288").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
        '    '        '                    nSum += oMatrix.Columns.Item("288").Cells.Item(iContTotal).Specific.value.ToString().Replace("QTZ", "").Replace("USD", "")
        '    '        '                End If

        '    '        '            End If

        '    '        '        End If

        '    '        '        'If form.Items.Item("3").Specific.value = "S" Then


        '    '        '        '    If Not String.IsNullOrEmpty(oMatrix.Columns.Item("11").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
        '    '        '        '        nSum += oMatrix.Columns.Item("11").Cells.Item(iContTotal).Specific.value.ToString().Replace("QTZ", "").Replace("USD", "")
        '    '        '        '    End If


        '    '        '        'Else

        '    '        '        '    If Not String.IsNullOrEmpty(oMatrix.Columns.Item("284").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
        '    '        '        '        nSum += oMatrix.Columns.Item("284").Cells.Item(iContTotal).Specific.value.ToString().Replace("QTZ", "").Replace("USD", "")
        '    '        '        '    End If


        '    '        '        'End If


        '    '        '        '   Else




        '    '        '        ' End If

        '    '        '    Next


        '    '        '    nTotal1 = nSum - form.Items.Item("29").Specific.value.ToString().Replace("QTZ", "").Replace("USD", "")


        '    '        '    If Not nTotal1 = 0 Then

        '    '        '        form.Freeze(True)

        '    '        '        oCheck = form.Items.Item("105").Specific

        '    '        '        oCheck.Checked = True

        '    '        '        form.Items.Item("103").Specific.value = nTotal1

        '    '        '        form.Items.Item("1").Click()

        '    '        '        form.Freeze(False)
        '    '        '    End If


        '    '        'End If


        '    '    End If






        'End Sub

        <B1Listener(BoEventTypes.et_CLICK, True)>
        Public Overridable Function OnBeforeClick(ByVal pVal As ItemEvent) As Boolean
            Dim form As Form = B1Connections.theAppl.Forms.Item(pVal.FormUID)
            Dim item As Item = form.Items.Item("1")
            Dim button As Button = CType(item.Specific, Button)
            'ADD YOUR ACTION CODE HERE ...

            Dim nSum As Decimal = 0.0
            Dim nTotal1 As Decimal = 0.0
            Dim nTotal2 As Decimal = 0.0
            Dim nTotalString As String = ""
            Dim nTotalresultado As String = ""


            Dim oMatrix As SAPbouiCOM.Matrix
            Dim oCheck As SAPbouiCOM.CheckBox

            oSerie = form.Items.Item("88").Specific.value.ToString().Trim()

            If form.Items.Item("3").Specific.value = "S" Then
                oMatrix = form.Items.Item("39").Specific
            Else
                oMatrix = form.Items.Item("38").Specific
            End If



            oRecordSetCancelacion = Nothing
            oRecordSetCancelacion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
            sQuery = "select T0.""IsForCncl"" from NNM1 T0 Where T0.""ObjectCode"" = 13 and T0.""Series"" = " & oSerie
            oRecordSetCancelacion.DoQuery(sQuery)
            oRecordSetCancelacion.MoveFirst()



            If oRecordSetCancelacion.Fields.Item(0).Value = "N" Then


                If form.Mode = BoFormMode.fm_ADD_MODE Then


                    ' If form.Items.Item("63").Enabled = True Then



                    For iContTotal As Integer = 1 To oMatrix.RowCount - 1


                        If form.Items.Item("63").Specific.value = "USD" Then

                            If form.Items.Item("3").Specific.value = "S" Then


                                If Not String.IsNullOrEmpty(oMatrix.Columns.Item("11").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
                                    nTotalString = oMatrix.Columns.Item("11").Cells.Item(iContTotal).Specific.value.ToString()
                                    nTotalresultado = nTotalString.Substring(0, 3)
                                    nSum += oMatrix.Columns.Item("11").Cells.Item(iContTotal).Specific.value.ToString().Replace(nTotalresultado, "").Replace("USD", "")
                                End If


                            Else

                                If Not String.IsNullOrEmpty(oMatrix.Columns.Item("284").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then

                                    nTotalString = oMatrix.Columns.Item("284").Cells.Item(iContTotal).Specific.value.ToString()
                                    nTotalresultado = nTotalString.Substring(0, 3)
                                    nSum += oMatrix.Columns.Item("284").Cells.Item(iContTotal).Specific.value.ToString().Replace(nTotalresultado, "").Replace("USD", "")
                                End If


                            End If


                        Else

                            If form.Items.Item("3").Specific.value = "S" Then

                                If Not String.IsNullOrEmpty(oMatrix.Columns.Item("187").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then
                                    nTotalString = oMatrix.Columns.Item("187").Cells.Item(iContTotal).Specific.value
                                    nTotalresultado = nTotalString.Substring(0, 3)
                                    nSum += oMatrix.Columns.Item("187").Cells.Item(iContTotal).Specific.value.Replace(nTotalresultado, "").Replace("USD", "").Replace(",", "")

                                End If

                            Else

                                If Not String.IsNullOrEmpty(oMatrix.Columns.Item("288").Cells.Item(iContTotal).Specific.value.ToString().Trim()) Then

                                    nTotalString = oMatrix.Columns.Item("288").Cells.Item(iContTotal).Specific.value.ToString()
                                    nTotalresultado = nTotalString.Substring(0, 3)
                                    nSum += oMatrix.Columns.Item("288").Cells.Item(iContTotal).Specific.value.ToString().Replace(nTotalresultado, "").Replace("USD", "").Replace(",", "")

                                End If

                            End If


                        End If





                    Next

                    nTotalString = form.Items.Item("29").Specific.value.ToString()
                    nTotalresultado = nTotalString.Substring(0, 3)
                    nTotal1 = nSum - form.Items.Item("29").Specific.value.ToString().Replace(nTotalresultado, "").Replace("USD", "").Replace(",", "")


                    If Not nTotal1 = 0 Then

                        form.Freeze(True)

                        oCheck = form.Items.Item("105").Specific

                        oCheck.Checked = True

                        form.Items.Item("103").Specific.value = nTotal1

                        'form.Items.Item("1").Click()

                        form.Freeze(False)
                    End If

                    'End If

                End If
            End If

            Return True
        End Function

    End Class
End Namespace
