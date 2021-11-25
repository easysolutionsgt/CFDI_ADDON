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
Imports Factura_Electronica.net.ingface.www
Imports System.Xml
Imports System.Net
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Drawing.Printing

Namespace Factura_Electronica
    Public Class Agregar_Electronica
        Inherits B1Event

        Public Sub New()
            MyBase.New
        End Sub
        <B1Listener(BoEventTypes.et_FORM_DATA_ADD, False)>
        Public Overridable Sub OnAfterFormDataAdd(ByVal pVal As BusinessObjectInfo)
            Dim ActionSuccess As Boolean = pVal.ActionSuccess
            Dim form As Form = B1Connections.theAppl.Forms.Item(pVal.FormUID)
            Dim sDocnum As String
            Dim oRecordSetValidaTran As SAPbobsCOM.Recordset
            Dim oRecorSetBuscaCae As SAPbobsCOM.Recordset

            'ADD YOUR ACTION CODE HERE ...
            form = B1Connections.theAppl.Forms.ActiveForm

            Try
                If (form.TypeEx.ToString().Replace("-", "") = "133") Then
                    oSerie = form.Items.Item("88").Specific.value.ToString().Trim()

                    oRecordSetCancelacion = Nothing
                    oRecordSetCancelacion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select T0.""IsForCncl"" from NNM1 T0 Where T0.""ObjectCode"" = 13 and T0.""Series"" = " & oSerie
                    oRecordSetCancelacion.DoQuery(sQuery)
                    oRecordSetCancelacion.MoveFirst()

                    oRecordSetValidaTran = Nothing
                    oRecordSetValidaTran = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""U_NomSerie"" from ""@SERIESO"" T0 where T0.""U_Document"" = '13' and T0.""U_Serie"" = " & oSerie
                    oRecordSetValidaTran.DoQuery(sQuery)
                    oRecordSetValidaTran.MoveFirst()

                    If Not String.IsNullOrEmpty(oRecordSetValidaTran.Fields.Item(0).Value) Then

                        sDocnum = form.Items.Item("8").Specific.value

                        oRecorSetBuscaCae = Nothing
                        oRecorSetBuscaCae = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select T0.""U_DocNum"" From OINV T0 Where T0.""DocNum"" = " & sDocnum
                        oRecorSetBuscaCae.DoQuery(sQuery)
                        oRecorSetBuscaCae.MoveFirst()


                        If oRecordSetCancelacion.Fields.Item(0).Value = "N" Then
                            Globales.ProcesoFacturaElectronica("13", sDocnum, form.Items.Item("88").Specific.value)
                        Else

                            Globales.ProcesoCancelacion("13", sDocnum, oRecorSetBuscaCae.Fields.Item(0).Value)
                        End If
                    End If
                ElseIf (form.TypeEx.ToString().Replace("-", "") = "60091") Then
                    oSerie = form.Items.Item("88").Specific.value.ToString().Trim()

                    oRecordSetCancelacion = Nothing
                    oRecordSetCancelacion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select T0.""IsForCncl"" from NNM1 T0 Where T0.""ObjectCode"" = 13 and T0.""Series"" = " & oSerie
                    oRecordSetCancelacion.DoQuery(sQuery)
                    oRecordSetCancelacion.MoveFirst()

                    oRecordSetValidaTran = Nothing
                    oRecordSetValidaTran = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""U_NomSerie"" from ""@SERIESO"" T0 where T0.""U_Document"" = '13' and T0.""U_Serie"" = " & oSerie
                    oRecordSetValidaTran.DoQuery(sQuery)

                    If Not String.IsNullOrEmpty(oRecordSetValidaTran.Fields.Item(0).Value) Then
                        sDocnum = form.Items.Item("8").Specific.value

                        oRecorSetBuscaCae = Nothing
                        oRecorSetBuscaCae = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select T0.""U_DocNum"" From OINV T0 Where T0.""DocNum"" = " & sDocnum
                        oRecorSetBuscaCae.DoQuery(sQuery)
                        oRecorSetBuscaCae.MoveFirst()

                        If oRecordSetCancelacion.Fields.Item(0).Value = "N" Then
                            Globales.ProcesoFacturaElectronica("13", sDocnum, form.Items.Item("88").Specific.value)
                        Else
                            Globales.ProcesoCancelacion("13", sDocnum, oRecorSetBuscaCae.Fields.Item(0).Value)
                        End If
                    End If

                ElseIf (form.TypeEx.ToString().Replace("-", "") = "179") Then

                    oSerie = form.Items.Item("88").Specific.value.ToString().Trim()

                    oRecordSetCancelacion = Nothing
                    oRecordSetCancelacion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select T0.""IsForCncl"" from NNM1 T0 Where T0.""ObjectCode"" = 14 and T0.""Series"" = " & oSerie
                    oRecordSetCancelacion.DoQuery(sQuery)
                    oRecordSetCancelacion.MoveFirst()

                    oRecordSetValidaTran = Nothing
                    oRecordSetValidaTran = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""U_NomSerie"" from ""@SERIESO"" T0 where T0.""U_Document"" = '14' and T0.""U_Serie"" = " & oSerie
                    oRecordSetValidaTran.DoQuery(sQuery)
                    oRecordSetValidaTran.MoveFirst()

                    If Not String.IsNullOrEmpty(oRecordSetValidaTran.Fields.Item(0).Value) Then
                        sDocnum = form.Items.Item("8").Specific.value

                        oRecorSetBuscaCae = Nothing
                        oRecorSetBuscaCae = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select T0.""U_DocNum"" From ORIN T0 Where T0.""DocNum"" = " & sDocnum
                        oRecorSetBuscaCae.DoQuery(sQuery)
                        oRecorSetBuscaCae.MoveFirst()

                        If oRecordSetCancelacion.Fields.Item(0).Value = "N" Then
                            Globales.ProcesoFacturaElectronica("14", sDocnum, form.Items.Item("88").Specific.value)
                        Else
                            Globales.ProcesoCancelacion("14", sDocnum, oRecorSetBuscaCae.Fields.Item(0).Value)
                        End If
                    End If
                ElseIf (form.TypeEx.ToString().Replace("-", "") = "65303") Then

                    oSerie = form.Items.Item("88").Specific.value.ToString().Trim()

                    oRecordSetCancelacion = Nothing
                    oRecordSetCancelacion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select T0.""IsForCncl"" from NNM1 T0 Where T0.""ObjectCode"" = 13 and T0.""DocSubType"" = 'DN' and T0.""Series"" = " & oSerie
                    oRecordSetCancelacion.DoQuery(sQuery)
                    oRecordSetCancelacion.MoveFirst()

                    oRecordSetValidaTran = Nothing
                    oRecordSetValidaTran = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""U_NomSerie"" from ""@SERIESO"" T0 where T0.""U_Document"" = 'DN' and T0.""U_Serie"" = " & oSerie
                    oRecordSetValidaTran.DoQuery(sQuery)

                    If Not String.IsNullOrEmpty(oRecordSetValidaTran.Fields.Item(0).Value) Then
                        sDocnum = form.Items.Item("8").Specific.value

                        oRecorSetBuscaCae = Nothing
                        oRecorSetBuscaCae = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select T0.""U_DocNum"" From OINV T0 Where T0.""DocSubType"" = 'DN' and  T0.""DocNum"" = " & sDocnum
                        oRecorSetBuscaCae.DoQuery(sQuery)
                        oRecorSetBuscaCae.MoveFirst()

                        If oRecordSetCancelacion.Fields.Item(0).Value = "N" Then
                            Globales.ProcesoFacturaElectronica("DN", sDocnum, form.Items.Item("88").Specific.value)
                        Else
                            Globales.ProcesoCancelacion("DN", sDocnum, oRecorSetBuscaCae.Fields.Item(0).Value)
                        End If
                    End If
                ElseIf (form.TypeEx.ToString().Replace("-", "") = "141") Or (form.TypeEx.ToString().Replace("-", "") = "60092") Then

                    oSerie = form.Items.Item("88").Specific.value.ToString().Trim()

                    oRecordSetCancelacion = Nothing
                    oRecordSetCancelacion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select T0.""IsForCncl"" from NNM1 T0 Where T0.""ObjectCode"" = 18 and T0.""Series"" = " & oSerie
                    oRecordSetCancelacion.DoQuery(sQuery)
                    oRecordSetCancelacion.MoveFirst()

                    oRecordSetValidaTran = Nothing
                    oRecordSetValidaTran = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""U_NomSerie"" from ""@SERIESO"" T0 where T0.""U_Document"" = '18' and T0.""U_Serie"" = " & oSerie
                    oRecordSetValidaTran.DoQuery(sQuery)

                    If Not String.IsNullOrEmpty(oRecordSetValidaTran.Fields.Item(0).Value) Then
                        sDocnum = form.Items.Item("8").Specific.value

                        oRecorSetBuscaCae = Nothing
                        oRecorSetBuscaCae = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select T0.""U_DocNum"" From OPCH T0 Where T0.""DocNum"" = " & sDocnum
                        oRecorSetBuscaCae.DoQuery(sQuery)
                        oRecorSetBuscaCae.MoveFirst()

                        If oRecordSetCancelacion.Fields.Item(0).Value = "N" Then
                            Globales.ProcesoFacturaElectronica("18", sDocnum, form.Items.Item("88").Specific.value)
                        Else
                            Globales.ProcesoCancelacion("18", sDocnum, oRecorSetBuscaCae.Fields.Item(0).Value)
                        End If
                    End If
                ElseIf (form.TypeEx.ToString().Replace("-", "") = "170") Then
                    sDocnum = form.Items.Item("3").Specific.value
                    'Globales.ProcesoFacturaElectronica("24", sDocnum)
                End If

            Catch ex As Exception
                B1Connections.theAppl.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Medium, True)
            End Try
        End Sub

        <B1Listener(BoEventTypes.et_FORM_DATA_UPDATE, False)>
        Public Overridable Sub OnAfterFormDataUpdate(ByVal pVal As BusinessObjectInfo)
            Dim ActionSuccess As Boolean = pVal.ActionSuccess
            Dim form As Form = B1Connections.theAppl.Forms.Item(pVal.FormUID)
            Dim sDocnum As String
            Dim oRecordSetValidaTran As SAPbobsCOM.Recordset
            Dim oRecordSetValidaEstado As SAPbobsCOM.Recordset
            'ADD YOUR ACTION CODE HERE ...
            form = B1Connections.theAppl.Forms.ActiveForm

            Try
                If form.TypeEx.ToString().Replace("-", "") = "133" Then
                    oSerie = form.Items.Item("88").Specific.value.ToString().Trim()

                    oRecordSetCancelacion = Nothing
                    oRecordSetCancelacion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select T0.""IsForCncl"" from NNM1 T0 Where T0.""ObjectCode"" = 13 and T0.""Series"" = " & oSerie
                    oRecordSetCancelacion.DoQuery(sQuery)
                    oRecordSetCancelacion.MoveFirst()

                    oRecordSetValidaTran = Nothing
                    oRecordSetValidaTran = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""U_NomSerie"" from ""@SERIESO"" T0 where T0.""U_Document"" = '13' and T0.""U_Serie"" = " & form.Items.Item("88").Specific.value.ToString().Trim()
                    oRecordSetValidaTran.DoQuery(sQuery)

                    If Not String.IsNullOrEmpty(oRecordSetValidaTran.Fields.Item(0).Value) Then
                        sDocnum = form.Items.Item("8").Specific.value

                        oRecordSetValidaEstado = Nothing
                        oRecordSetValidaEstado = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select * From OINV Where ""DocSubType"" <> 'DN' and ""DocNum"" = " & sDocnum
                        oRecordSetValidaEstado.DoQuery(sQuery)

                        If oRecordSetValidaEstado.RecordCount > 0 Then
                            If oRecordSetValidaEstado.Fields.Item("U_Valido").Value <> "true" Then
                                If oRecordSetCancelacion.Fields.Item(0).Value = "N" Then
                                    Globales.ProcesoFacturaElectronica("13", sDocnum, form.Items.Item("88").Specific.value)
                                Else
                                    Globales.ProcesoCancelacion("13", sDocnum, oRecordSetValidaEstado.Fields.Item(0).Value)
                                End If
                            End If
                        End If
                    End If
                ElseIf (form.TypeEx.ToString().Replace("-", "") = "60091") Then
                    oSerie = form.Items.Item("88").Specific.value.ToString().Trim()

                    oRecordSetCancelacion = Nothing
                    oRecordSetCancelacion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select T0.""IsForCncl"" from NNM1 T0 Where T0.""ObjectCode"" = 13 and T0.""Series"" = " & oSerie
                    oRecordSetCancelacion.DoQuery(sQuery)
                    oRecordSetCancelacion.MoveFirst()

                    oRecordSetValidaTran = Nothing
                    oRecordSetValidaTran = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""U_NomSerie"" from ""@SERIESO"" T0 where T0.""U_Document"" = '13' and T0.""U_Serie"" = " & oSerie
                    oRecordSetValidaTran.DoQuery(sQuery)

                    If Not String.IsNullOrEmpty(oRecordSetValidaTran.Fields.Item(0).Value) Then
                        sDocnum = form.Items.Item("8").Specific.value

                        oRecordSetValidaEstado = Nothing
                        oRecordSetValidaEstado = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select * From OINV Where ""DocSubType"" <> 'DN' and ""DocNum"" = " & sDocnum
                        oRecordSetValidaEstado.DoQuery(sQuery)

                        If oRecordSetValidaEstado.RecordCount > 0 Then
                            If oRecordSetValidaEstado.Fields.Item("U_Valido").Value <> "true" Then
                                If oRecordSetCancelacion.Fields.Item(0).Value = "N" Then
                                    Globales.ProcesoFacturaElectronica("13", sDocnum, form.Items.Item("88").Specific.value)
                                Else
                                    If B1Connections.diCompany.DbServerType = BoDataServerTypes.dst_HANADB Then
                                        Globales.ProcesoCancelacion("13", sDocnum, oRecordSetValidaEstado.Fields.Item("U_DocNum").Value)
                                    Else
                                        Globales.ProcesoCancelacion("13", sDocnum, oRecordSetValidaEstado.Fields.Item("U_DocNum").Value)
                                    End If
                                End If
                            End If
                        End If
                    End If
                ElseIf (form.TypeEx.ToString() = "141") Or (form.TypeEx = "60092") Then

                    oSerie = form.Items.Item("88").Specific.value.ToString().Trim()

                    oRecordSetCancelacion = Nothing
                    oRecordSetCancelacion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select T0.""IsForCncl"" from NNM1 T0 Where T0.""ObjectCode"" = 18 and T0.""Series"" = " & oSerie
                    oRecordSetCancelacion.DoQuery(sQuery)
                    oRecordSetCancelacion.MoveFirst()

                    oRecordSetValidaTran = Nothing
                    oRecordSetValidaTran = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""U_NomSerie"" from ""@SERIESO"" T0 where T0.""U_Document"" = '18' and T0.""U_Serie"" = " & oSerie
                    oRecordSetValidaTran.DoQuery(sQuery)

                    If Not String.IsNullOrEmpty(oRecordSetValidaTran.Fields.Item(0).Value) Then
                        sDocnum = form.Items.Item("8").Specific.value

                        oRecordSetValidaEstado = Nothing
                        oRecordSetValidaEstado = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select * From OPCH Where ""DocNum"" = " & sDocnum
                        oRecordSetValidaEstado.DoQuery(sQuery)
                        If oRecordSetValidaEstado.RecordCount > 0 Then
                            If oRecordSetValidaEstado.Fields.Item("U_Valido").Value <> "true" Then
                                If oRecordSetCancelacion.Fields.Item(0).Value = "N" Then
                                    Globales.ProcesoFacturaElectronica("18", sDocnum, form.Items.Item("88").Specific.value)
                                Else
                                    If B1Connections.diCompany.DbServerType = BoDataServerTypes.dst_HANADB Then
                                        Globales.ProcesoCancelacion("18", sDocnum, oRecordSetValidaEstado.Fields.Item("U_DocNum").Value)
                                    Else
                                        Globales.ProcesoCancelacion("18", sDocnum, oRecordSetValidaEstado.Fields.Item("U_DocNum").Value)
                                    End If
                                End If
                            End If
                        End If
                    End If
                ElseIf (form.TypeEx.ToString().Replace("-", "") = "179") Then
                    oSerie = form.Items.Item("88").Specific.value.ToString().Trim()

                    oRecordSetCancelacion = Nothing
                    oRecordSetCancelacion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select T0.""IsForCncl"" from NNM1 T0 Where T0.""ObjectCode"" = 14 and T0.""Series"" = " & oSerie
                    oRecordSetCancelacion.DoQuery(sQuery)
                    oRecordSetCancelacion.MoveFirst()

                    oRecordSetValidaTran = Nothing
                    oRecordSetValidaTran = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""U_NomSerie"" from ""@SERIESO"" T0 where T0.""U_Document"" = '14' and T0.""U_Serie"" = " & oSerie
                    oRecordSetValidaTran.DoQuery(sQuery)

                    If Not String.IsNullOrEmpty(oRecordSetValidaTran.Fields.Item(0).Value) Then
                        sDocnum = form.Items.Item("8").Specific.value

                        oRecordSetValidaEstado = Nothing
                        oRecordSetValidaEstado = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select * From ORIN Where ""DocNum"" = " & sDocnum
                        oRecordSetValidaEstado.DoQuery(sQuery)

                        If oRecordSetValidaEstado.RecordCount > 0 Then
                            If oRecordSetValidaEstado.Fields.Item("U_Valido").Value <> "true" Then
                                If oRecordSetCancelacion.Fields.Item(0).Value = "N" Then
                                    Globales.ProcesoFacturaElectronica("14", sDocnum, form.Items.Item("88").Specific.value)
                                Else
                                    If B1Connections.diCompany.DbServerType = BoDataServerTypes.dst_HANADB Then
                                        Globales.ProcesoCancelacion("14", sDocnum, oRecordSetValidaEstado.Fields.Item("U_DocNum").Value)
                                    Else
                                        Globales.ProcesoCancelacion("14", sDocnum, oRecordSetValidaEstado.Fields.Item("U_DocNum").Value)
                                    End If
                                End If
                            End If
                        End If
                    End If
                ElseIf (form.TypeEx.ToString().Replace("-", "") = "65303") Then
                    oSerie = form.Items.Item("88").Specific.value.ToString().Trim()

                    oRecordSetCancelacion = Nothing
                    oRecordSetCancelacion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select T0.""IsForCncl"" from NNM1 T0 Where T0.""ObjectCode"" = 13 and T0.""DocSubType"" = 'DN' and T0.""Series"" = " & oSerie
                    oRecordSetCancelacion.DoQuery(sQuery)
                    oRecordSetCancelacion.MoveFirst()

                    oRecordSetValidaTran = Nothing
                    oRecordSetValidaTran = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""U_NomSerie"" from ""@SERIESO"" T0 where T0.""U_Document"" = 'DN' and T0.""U_Serie"" = " & oSerie
                    oRecordSetValidaTran.DoQuery(sQuery)

                    If Not String.IsNullOrEmpty(oRecordSetValidaTran.Fields.Item(0).Value) Then
                        sDocnum = form.Items.Item("8").Specific.value

                        oRecordSetValidaEstado = Nothing
                        oRecordSetValidaEstado = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select * From OINV Where ""DocSubType"" = 'DN' and ""DocNum"" = " & sDocnum
                        oRecordSetValidaEstado.DoQuery(sQuery)
                        If oRecordSetValidaEstado.RecordCount > 0 Then
                            If oRecordSetValidaEstado.Fields.Item("U_Valido").Value <> "true" Then
                                If oRecordSetCancelacion.Fields.Item(0).Value = "N" Then
                                    Globales.ProcesoFacturaElectronica("DN", sDocnum, form.Items.Item("88").Specific.value)
                                Else
                                    If B1Connections.diCompany.DbServerType = BoDataServerTypes.dst_HANADB Then
                                        Globales.ProcesoCancelacion("DN", sDocnum, oRecordSetValidaEstado.Fields.Item("U_DocNum").Value)
                                    Else
                                        Globales.ProcesoCancelacion("DN", sDocnum, oRecordSetValidaEstado.Fields.Item("U_DocNum").Value)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                B1Connections.theAppl.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Medium, True)
            End Try
        End Sub
    End Class
End Namespace