Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Xml
Imports B1WizardBase
Imports Newtonsoft.Json
Imports SAPbobsCOM
Imports SAPbouiCOM
Imports System.Text
Imports System.Security.Cryptography
Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports Factura_Electronica.wsFirmaFel


Module Globales
    Public sXML As String = ""
    Public correos As New MailMessage
    Public envios As New SmtpClient
    Public oProgBar As SAPbouiCOM.ProgressBar
    Public iContProgBar As Integer = 0
    Public IContName As Integer
    Public LineaCuentaAjena As Integer
    Public ProcesaCuentaAjena As String
    Public SumaDescuentos As Decimal = 0
    Public SumaExento As Decimal = 0
    Public SumaNeto As Decimal = 0
    Public SumaLineasTotal As Decimal = 0
    Public SumaBruto As Decimal = 0
    Public SumaIva As Decimal = 0
    Public TotalAgrupado As Double = 0
    Public ImpuestoAgrupado As Double = 0
    Public GravableAgrupado As Double = 0
    Public sQuery As String = ""
    Public TipoSap As String = ""
    Public TipoDTE As String = ""
    Public AreaName As String = ""
    Public oRecordSetH As SAPbobsCOM.Recordset
    Public oRecordSetGAdicionales As SAPbobsCOM.Recordset
    Public oRecordRegimenA As SAPbobsCOM.Recordset
    Public skey As String
    Public sLllaves As String
    Public x As String = ""
    Public index As Integer = 0
    Public oFilters As SAPbouiCOM.EventFilters
    Public oFilter As SAPbouiCOM.EventFilter
    Public sDocNum As String
    Public oRecordSetPH As Recordset
    Public oRecordSetSERIESO As SAPbobsCOM.Recordset
    Public oRecordSetPD As Recordset
    Public PH As String
    Public PD As String
    Public oRecordsetMapeoH As SAPbobsCOM.Recordset
    Public oRecordsetQUERYD As SAPbobsCOM.Recordset
    Public oRecordsetArticulo As SAPbobsCOM.Recordset
    Public oRecordSetCancelacion As SAPbobsCOM.Recordset
    Public oArchivo As StreamWriter
    Public oArchivo2 As StreamWriter
    Public oRecordSetFEL As SAPbobsCOM.Recordset
    Public oRecordSetNitV As SAPbobsCOM.Recordset
    Public oRecordSetCorreoEmisor As SAPbobsCOM.Recordset
    Public oRecordSetUpdate As SAPbobsCOM.Recordset
    Public oRecordSetUpdateNum As SAPbobsCOM.Recordset
    Public oRutaPdf As String = ""
    Public oXML As New XmlDocument
    Public oXMLRet As New XmlDocument
    Public oXMLInterno As New XmlDocument
    Public dsGenerarGuia As New DataSet()
    Public dsGenerarGuia2 As New DataSet()
    Public dsGenerarGuia3 As New DataSet()
    Public dsGenerarOUT As New DataSet()
    Public HttpReq As HttpWebRequest
    Public UrlPDF As String = ""
    Public oSerie As String = ""
    Public TipoEnvio As String = ""
    Public AreaCliente As String = ""
    Public oDocumento As SAPbobsCOM.Documents
    Public iValida As Integer = 0
    Public Texto As String
    Public Numero As String
    Public NumeroR As String
    Public serieR As String
    Public oRegimenISR As String = ""
    Public UrlEnvio As String
    Public Subtotal1 As Double = 0.00
    Public Subtotal2 As Double = 0.00


    Dim xmlNodeRdr As XmlNodeReader
    Dim xmlNodeRdr2 As XmlNodeReader
    Dim xmlNodeRdr3 As XmlNodeReader

    'Variables de respuesta WS
    Dim numeroAutorizacion As String
    Dim Serie As String
    Dim PreImpreso As String
    Dim Documento As String
    Dim nombre As String
    Dim Direccion As String
    Dim Mensaje As String
    Dim NIT As String
    Dim Certificador As String
    Dim TimeStamp As String
    Public sDocEntry As String
    Dim lErrCode As Integer = 0
    Dim sErrMsg As String = ""

    Public Function Validar_Licencia(ByVal xkey As String) As Boolean

        Try
            Dim sTexto As String = ""
            Dim sFecha As Date
            Dim oRecordSetLicencia As SAPbobsCOM.Recordset
            Dim stemp As String = ""
            Dim sFecha2 As Date

            oRecordSetLicencia = Nothing
            oRecordSetLicencia = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
            sQuery = "Select T0.""U_Licencia"" from  ""@CNFGLIC"" T0 where T0.""Code"" = 1 "
            oRecordSetLicencia.DoQuery(sQuery)
            oRecordSetLicencia.MoveFirst()
            sLllaves = Globales.Desencriptar(oRecordSetLicencia.Fields.Item(0).Value)

            index = sLllaves.IndexOf("|")

            sLllaves = sLllaves.Substring(9, index - 9)


            If (sLllaves = xkey) Then
                sTexto = Globales.Desencriptar(oRecordSetLicencia.Fields.Item(0).Value)
                sFecha = Date.Parse(Globales.Desencriptar(oRecordSetLicencia.Fields.Item(0).Value).Substring(0, 2) & "/" & Globales.Desencriptar(oRecordSetLicencia.Fields.Item(0).Value).Substring(2, 2) & "/" & Globales.Desencriptar(oRecordSetLicencia.Fields.Item(0).Value).Substring(4, 4))


                Dim index As Integer = sTexto.IndexOf("|")
                sLllaves = sTexto.Substring(9, index - 9)
                stemp = Globales.Desencriptar(oRecordSetLicencia.Fields.Item(0).Value).Substring(10 + sLllaves.Length, 8)
                sFecha2 = Date.Parse(stemp.Substring(0, 2) & "/" & stemp.Substring(2, 2) & "/" & stemp.Substring(4, 4))

                If Now.Date >= sFecha2 Then
                    If Now.Date = sFecha2 Then
                        B1Connections.theAppl.MessageBox("ADVERTENCIA: El Día De Hoy Se Vence la licencia....")
                    Else
                        B1Connections.theAppl.MessageBox("La Licencia Vencio, Contacte A Su Proveedor....")
                        Return False
                    End If
                End If

                If Now.Date < sFecha Then
                    System.Windows.Forms.MessageBox.Show("ADVERTENCIA: Aun No Cumple Con la fecha inicial....")
                    Return False
                End If

                If Now.Date > sFecha And Now.Date < sFecha2 Then

                    If DateDiff(DateInterval.Day, Now.Date, sFecha2) = 30 Then
                        B1Connections.theAppl.MessageBox("ADVERTENCIA: La Licencia Vencera En Un Mes")
                    ElseIf DateDiff(DateInterval.Day, Now.Date, sFecha2) <= 7 Then
                        B1Connections.theAppl.MessageBox("ADVERTENCIA: La Licencia Vencera En: " & DateDiff(DateInterval.Day, Now.Date, sFecha2) & " Días")
                    End If

                End If
            Else
                'System.Windows.Forms.MessageBox.Show("Hardware Key No Valido...")
                Return False
            End If
            Return True
        Catch ex As Exception
            B1Connections.theAppl.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Medium, True)
            'MsgBox(ex.Message)
        End Try
    End Function

    Public Sub Mostrar_PDF(ByRef TipoDoc As String, ByRef sSerie As String, ByRef sNumDoc As String)

        Dim oRecordSetCnfgFace As SAPbobsCOM.Recordset
        Dim oRecordSetFactura As SAPbobsCOM.Recordset

        Dim sQuery As String = ""
        Dim oRutaPdf As String
        Dim oNumFac As String = ""

        Dim form As Form = B1Connections.theAppl.Forms.ActiveForm

        Try

            oRutaPdf = ""
            oNumFac = ""
            sQuery = ""

            form = B1Connections.theAppl.Forms.ActiveForm

            oRecordSetCnfgFace = Nothing
            oRecordSetCnfgFace = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
            sQuery = "select * from ""@SERIESO"" T0 Where T0.""U_Serie"" = '" & sSerie.Trim().TrimEnd().TrimStart() & "'"
            oRecordSetCnfgFace.DoQuery(sQuery)
            oRecordSetCnfgFace.MoveFirst()

            If Not String.IsNullOrEmpty(oRecordSetCnfgFace.Fields.Item("U_RutaPDF").Value.ToString()) Then

                oRecordSetFactura = Nothing
                oRecordSetFactura = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

                If TipoDoc = "133" Or TipoDoc = "60091" Then
                    sQuery = "select * from OINV T0 where T0.""DocSubType"" <> 'DN' And T0. ""DocNum"" = '" & sNumDoc & "' And T0.""Series"" = " & sSerie
                    oRecordSetFactura.DoQuery(sQuery)
                    oRecordSetFactura.MoveFirst()
                    If form.Items.Item("81").Specific.value = "4" Then
                        Exit Try
                    Else
                        oNumFac = oRecordSetFactura.Fields.Item("U_DocNum").Value.ToString()
                        oRutaPdf = oRecordSetCnfgFace.Fields.Item("U_RutaPDF").Value.ToString() & "\" & oRecordSetFactura.Fields.Item("Series").Value.ToString() & "_Factura_" & oNumFac & ".PDF"
                    End If
                ElseIf TipoDoc = "141" Then
                    sQuery = "select * from OPCH T0 where T0. ""DocNum"" = '" & sNumDoc & "' And T0.""Series"" = " & sSerie
                    oRecordSetFactura.DoQuery(sQuery)
                    oRecordSetFactura.MoveFirst()
                    If form.Items.Item("81").Specific.value = "4" Then
                        Exit Try
                    Else
                        oNumFac = oRecordSetFactura.Fields.Item("U_DocNum").Value.ToString()
                        oRutaPdf = oRecordSetCnfgFace.Fields.Item("U_RutaPDF").Value.ToString() & "\" & oRecordSetFactura.Fields.Item("Series").Value.ToString() & "_FacturaEspecial_" & oNumFac & ".PDF"
                    End If
                ElseIf TipoDoc = "179" Then
                    sQuery = "select * from ORIN T0 where T0. ""DocNum"" = '" & sNumDoc & "' And T0.""Series"" = " & sSerie
                    oRecordSetFactura.DoQuery(sQuery)
                    oRecordSetFactura.MoveFirst()
                    If form.Items.Item("81").Specific.value = "4" Then
                        Exit Try
                    Else
                        oNumFac = oRecordSetFactura.Fields.Item("U_DocNum").Value.ToString()
                        oRutaPdf = oRecordSetCnfgFace.Fields.Item("U_RutaPDF").Value.ToString() & "\" & oRecordSetFactura.Fields.Item("Series").Value.ToString() & "_NotaCredito_" & oNumFac & ".PDF"
                    End If
                ElseIf TipoDoc = "141" Then
                    sQuery = "select * from OPCH T0 where T0. ""DocNum"" = '" & sNumDoc & "' And T0.""Series"" = " & sSerie
                    oRecordSetFactura.DoQuery(sQuery)
                    oRecordSetFactura.MoveFirst()
                    If form.Items.Item("81").Specific.value = "4" Then
                        Exit Try
                    Else
                        oNumFac = oRecordSetFactura.Fields.Item("U_DocNum").Value.ToString()
                        oRutaPdf = oRecordSetCnfgFace.Fields.Item("U_RutaPDF").Value.ToString() & "\" & oRecordSetFactura.Fields.Item("Series").Value.ToString() & "_FacturaEspecial_" & oNumFac & ".PDF"
                    End If
                ElseIf TipoDoc = "65303" Then
                    sQuery = "select * from OINV T0 where T0.""DocSubType"" = 'DN' And T0. ""DocNum"" = '" & sNumDoc & "' And T0.""Series"" = " & sSerie
                    oRecordSetFactura.DoQuery(sQuery)
                    oRecordSetFactura.MoveFirst()

                    sQuery = "select * from OINV T0 where T0. ""DocNum"" = '" & sNumDoc & "' And T0.""Series"" = " & sSerie
                    oRecordSetFactura.DoQuery(sQuery)
                    oRecordSetFactura.MoveFirst()

                    If form.Items.Item("81").Specific.value = "4" Then
                        System.Diagnostics.Process.Start("https://report.feel.com.gt/ingfacereport/ingfacereport_documento?uuid=" & oRecordSetFactura.Fields.Item("U_CAE").Value)
                        Exit Try
                    Else
                        oNumFac = oRecordSetFactura.Fields.Item("U_DocNum").Value.ToString()
                        oRutaPdf = oRecordSetCnfgFace.Fields.Item("U_RutaPDF").Value.ToString() & "\" & oRecordSetFactura.Fields.Item("Series").Value.ToString() & "_NotaDebito_" & oNumFac & ".PDF"
                    End If
                ElseIf TipoDoc = "170" Then
                    oRutaPdf = oRecordSetCnfgFace.Fields.Item("U_RutaPDF").Value.ToString() & "\" & oRecordSetFactura.Fields.Item("Series").Value.ToString() & "_Recibo_" & oNumFac & ".PDF"
                End If

                If oNumFac = "" And Not form.Items.Item("81").Specific.value = "4" Then
                    B1Connections.theAppl.SetStatusBarMessage("No Se Ha Generado Un PDF Para Este Documento", SAPbouiCOM.BoMessageTime.bmt_Short, True)
                Else
                    Process.Start(oRutaPdf)
                End If
            Else
                B1Connections.theAppl.SetStatusBarMessage("Configure Una Ruta Para Los Documentos PDF", SAPbouiCOM.BoMessageTime.bmt_Short, True)
            End If
        Catch ex As Exception
            B1Connections.theAppl.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, True)
        End Try
    End Sub
    Public Function Query_Manager_H(ByVal sEtiquetaa As String, ByVal sDocumento As String, ByVal sTipo As String) As String

        Dim oResultado As String = ""

        oRecordsetMapeoH = Nothing
        oRecordsetMapeoH = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
        sQuery = "Select * From ""@QUERYH"" T0 Where T0.""U_Etiqueta"" = '" & sEtiquetaa & "' and T0.""U_Document"" = '" & sTipo & "'"
        oRecordsetMapeoH.DoQuery(sQuery)
        oRecordsetMapeoH.MoveFirst()

        If oRecordsetMapeoH.RecordCount > 0 Then

            sQuery = oRecordsetMapeoH.Fields.Item("U_Query").Value

            oRecordsetMapeoH = Nothing
            oRecordsetMapeoH = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
            sQuery = sQuery.Replace("¿?", sDocumento)
            oRecordsetMapeoH.DoQuery(sQuery)
            oRecordsetMapeoH.MoveFirst()

            If oRecordsetMapeoH.RecordCount > 0 Then
                oResultado = oRecordsetMapeoH.Fields.Item(0).Value
            End If
        End If
        Return oResultado
    End Function
    Public Function Query_Manager_D(ByVal sEtiquetaa As String, ByVal sDocumento As String, ByVal sLinea As String, ByVal sTipo As String) As String

        Dim oResultado As String = ""

        oRecordsetMapeoH = Nothing
        oRecordsetMapeoH = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
        sQuery = "Select * From ""@QUERYD"" T0 Where T0.""U_Etiqueta"" = '" & sEtiquetaa & "' and T0.""U_Document"" = '" & sTipo & "'"
        oRecordsetMapeoH.DoQuery(sQuery)
        oRecordsetMapeoH.MoveFirst()


        If oRecordsetMapeoH.RecordCount > 0 Then

            sQuery = oRecordsetMapeoH.Fields.Item("U_Query").Value

            oRecordsetMapeoH = Nothing
            oRecordsetMapeoH = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
            sQuery = sQuery.Replace("¿?", sDocumento)
            sQuery = sQuery.Replace("¡!", sLinea)
            oRecordsetMapeoH.DoQuery(sQuery)
            oRecordsetMapeoH.MoveFirst()

            If oRecordsetMapeoH.RecordCount > 0 Then
                oResultado = oRecordsetMapeoH.Fields.Item(0).Value
            End If

        End If
        Return oResultado
    End Function
    Public Function Query_Manager_P(ByVal sEtiquetaa As String, ByVal sDocumento As String, ByVal sLinea As String, ByVal sTipo As String) As String

        Dim oResultado As String = ""

        oRecordsetMapeoH = Nothing
        oRecordsetMapeoH = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
        sQuery = "Select * From ""@QUERYP"" T0 Where T0.""U_Etiqueta"" = '" & sEtiquetaa & "' and T0.""U_Document"" = '" & sTipo & "'"
        oRecordsetMapeoH.DoQuery(sQuery)
        oRecordsetMapeoH.MoveFirst()


        If oRecordsetMapeoH.RecordCount > 0 Then

            sQuery = oRecordsetMapeoH.Fields.Item("U_Query").Value

            oRecordsetMapeoH = Nothing
            oRecordsetMapeoH = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
            sQuery = sQuery.Replace("¿?", sDocumento)
            sQuery = sQuery.Replace("¡!", sLinea)
            oRecordsetMapeoH.DoQuery(sQuery)
            oRecordsetMapeoH.MoveFirst()

            If oRecordsetMapeoH.RecordCount > 0 Then
                ' MsgBox(oRecordsetMapeoH.Fields.Item(0).Value)
                oResultado = oRecordsetMapeoH.Fields.Item(0).Value
                'MsgBox(oResultado)
            End If

        End If
        Return oResultado
    End Function
    Public Sub SetFilters()

        '// Create a new EventFilters object
        oFilters = New SAPbouiCOM.EventFilters()

        '// add an event type to the container
        '// this method returns an EventFilter object
        oFilter = oFilters.Add(SAPbouiCOM.BoEventTypes.et_CLICK)

        '// assign the form type on which the event would be processed
        oFilter.AddEx("ftTraslados01") 'Orders Form

        oFilter = oFilters.Add(SAPbouiCOM.BoEventTypes.et_KEY_DOWN)

    End Sub
    Public Sub ProcesoCancelacion(ByRef TipoDoc As String, ByRef DocNum As String, ByRef oAntes As String)
        Try
            oRecordSetH = Nothing
            oRecordSetH = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

            If TipoDoc = "13" Then
                sQuery = "Select T1.*,T0.""E_Mail"",T1.""U_FacNit"",T0.""Phone1"" From OINV T1 inner join OCRD T0 On T0.""CardCode"" = T1.""CardCode"" Where T1.""DocSubType"" <> 'DN' and T1.""DocNum"" = " & DocNum
            ElseIf (TipoDoc = "14") Then
                sQuery = "Select T1.*,T0.""E_Mail"",T1.""U_FacNit"",T0.""Phone1"" From ORIN T1 inner join OCRD T0 On T0.""CardCode"" = T1.""CardCode"" Where T1.""DocNum"" = " & DocNum
            ElseIf (TipoDoc = "18") Then
                sQuery = "Select T1.*,T0.""E_Mail"",T1.""U_FacNit"",T0.""Phone1"" From OPCH T1 inner join OCRD T0 On T0.""CardCode"" = T1.""CardCode"" Where T1.""DocNum"" = " & DocNum
            ElseIf (TipoDoc = "DN") Then
                sQuery = "Select T1.*,T0.""E_Mail"",T1.""U_FacNit"",T0.""Phone1"" From OINV T1 inner join OCRD T0 On T0.""CardCode"" = T1.""CardCode"" Where T1.""DocSubType"" = 'DN' And T1.""DocNum"" = " & DocNum
            ElseIf (TipoDoc = "24") Then
                sQuery = "Select T1.*,T0.""E_Mail"",T0.""Phone1"",T0.""U_NIT"" From ORCT T1 inner join OCRD T0 On T0.""CardCode"" = T1.""CardCode"" Where T1.""DocNum"" = " & DocNum
            End If

            oRecordSetH.DoQuery(sQuery)

            If Not String.IsNullOrEmpty(oRecordSetH.Fields.Item("CardCode").Value) Then
                oRecordSetSERIESO = Nothing
                oRecordSetSERIESO = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                If TipoDoc = "24" Then
                    sQuery = "Select * from ""@SERIESO"" where ""U_Serie"" = '" & oRecordSetH.Fields.Item("Series").Value & "' and ""U_Document"" = '" & oRecordSetH.Fields.Item("ObjType").Value & "' "
                Else
                    If oRecordSetH.Fields.Item("DocSubType").Value = "DN" Then
                        sQuery = "Select * from ""@SERIESO"" where ""U_Serie"" = '" & oRecordSetH.Fields.Item("Series").Value & "' and ""U_Document"" = 'DN' "
                    Else
                        sQuery = "Select * from ""@SERIESO"" where ""U_Serie"" = '" & oRecordSetH.Fields.Item("Series").Value & "' and ""U_Document"" = '" & oRecordSetH.Fields.Item("ObjType").Value & "' "
                    End If
                End If

                oRecordSetSERIESO.DoQuery(sQuery)
                oRecordSetSERIESO.MoveFirst()

                If Not String.IsNullOrEmpty(oRecordSetSERIESO.Fields.Item("Object").Value) Then

                    oRecordSetFEL = Nothing
                    oRecordSetFEL = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select * from ""@CONFEL"" "
                    oRecordSetFEL.DoQuery(sQuery)

                    If oRecordSetFEL.RecordCount = 0 Then
                        B1Connections.theAppl.MessageBox("No Existe Ruta Para Depositar Los Archivos..")
                        Exit Try
                    End If

                    If TipoDoc = "13" Then
                        If System.IO.File.Exists(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\FacturaCancelacion_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml") Then
                            'The file exists
                            System.IO.File.Delete(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\FacturaCancelacion_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")
                        End If
                        oArchivo = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\FacturaCancelacion_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")
                    ElseIf TipoDoc = "18" Then
                        If System.IO.File.Exists(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\FacturaEspecialCancelacion_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml") Then
                            'The file exists
                            System.IO.File.Delete(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\FacturaEspecialCancelacion_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")
                        End If

                        oArchivo = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\FacturaEspecialCancelacion_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")

                    ElseIf (TipoDoc = "14") Then

                        If System.IO.File.Exists(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\NotaCreditoCancelacion_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml") Then
                            'The file exists
                            System.IO.File.Delete(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\NotaCreditoCancelacion_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")
                        End If

                        oArchivo = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\NotaCreditoCancelacion_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")

                    ElseIf (TipoDoc = "DN") Then

                        If System.IO.File.Exists(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\NotaDebitoCancelacion_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml") Then
                            'The file exists
                            System.IO.File.Delete(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\NotaDebitoCancelacion_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")
                        End If

                        oArchivo = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\NotaDebitoCancelacion_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")

                    ElseIf (TipoDoc = "24") Then
                        oArchivo = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\ReciboCancelacion_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")
                    End If

                    Dim sFechaDoc As String = Convert.ToDateTime(oRecordSetH.Fields.Item("DocDate").Value).ToString("yyyy-MM-ddTHH:mm:ss-06:00")
                    Dim sFechaDoc2 As String = Convert.ToDateTime(Now.Date).ToString("yyyy-MM-ddTHH:mm:ss-06:00")

                    oRecordSetNitV = Nothing
                    oRecordSetNitV = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""TaxIdNum"" from OADM T0"
                    oRecordSetNitV.DoQuery(sQuery)
                    oRecordSetNitV.MoveFirst()

                    sXML = "<?xml version=""1.0"" encoding=""utf-8""?>"
                    sXML &= " <dte:GTAnulacionDocumento xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"" xmlns:dte=""http://www.sat.gob.gt/dte/fel/0.1.0"" xmlns:n1=""http://www.altova.com/samplexml/other-namespace"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" Version=""0.1"" xsi:schemaLocation=""http://www.sat.gob.gt/dte/fel/0.1.0"">  "
                    sXML &= "       <dte:SAT>"
                    sXML &= "       <dte:AnulacionDTE ID=""DatosCertificados""> "
                    sXML &= "           <dte:DatosGenerales ID=""DatosAnulacion"" NumeroDocumentoAAnular=" & Chr(34) & oRecordSetH.Fields.Item("U_CAE").Value & Chr(34) & " NITEmisor=" & Chr(34) & oRecordSetNitV.Fields.Item(0).Value.ToString().Replace("-", "") & Chr(34) & " MotivoAnulacion=""Anulación"" IDReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_FacNit").Value.Replace("-", "") & Chr(34) & " FechaHoraAnulacion=" & Chr(34) & sFechaDoc2 & Chr(34) & " FechaEmisionDocumentoAnular=" & Chr(34) & sFechaDoc & Chr(34) & "/>"
                    sXML &= "   </dte:AnulacionDTE>"
                    sXML &= "       </dte:SAT>"
                    sXML &= "</dte:GTAnulacionDocumento>"

                    oArchivo.WriteLine(sXML)
                    oArchivo.Flush()
                    oArchivo.Close()

                    Dim Area As String
                    Area = oRecordSetFEL.Fields.Item("U_AreaName").Value

                    PostXMLAnul(sXML, "ANUL", TipoDoc, Area)
                End If
            End If
        Catch ex As Exception
            B1Connections.theAppl.MessageBox(ex.Message)
        End Try
    End Sub

    Public Sub ProcesoFacturaElectronica(ByRef TipoDoc As String, ByRef DocNum As String, ByRef Series As Integer)

        Dim AgrupaDetalle As String = ""
        Dim oRecordSetTipoLinea As SAPbobsCOM.Recordset
        Dim oRecordSetDecimales As SAPbobsCOM.Recordset
        Dim oRecordSetCnfgFace As SAPbobsCOM.Recordset
        Dim oRecordSetD As SAPbobsCOM.Recordset
        Dim oRecordSetAgrupa As SAPbobsCOM.Recordset
        Dim DescuentoLinea As Double
        Dim DescuentoTotalLinea As Double
        Dim PrecioLinea As Double
        Dim CantidadLinea As Double
        Dim oRecordSetCodigoEmpresa As SAPbobsCOM.Recordset
        Dim NumeroResolucion As String = ""
        Dim FechaResolucion As Date
        Dim oReferencia As String = ""
        Dim pPrecio As Decimal
        Dim pPrecioUni As Decimal
        Dim dQty As Decimal
        Dim pMontoG As Decimal
        Dim pIVA As Decimal
        Dim nCae As String = ""
        Dim nFac As String = ""
        Dim nSer As String = ""
        Dim nVal As String = ""
        Dim oRecordSetDepartamento As SAPbobsCOM.Recordset
        Dim oRecordSetDepartamentoV As SAPbobsCOM.Recordset
        Dim oRecordSetPais As SAPbobsCOM.Recordset
        Dim oRecordSetDireccion As SAPbobsCOM.Recordset
        Dim oRecordSetTaxCodeH As SAPbobsCOM.Recordset
        Dim oRecordSetCodigoPostal As SAPbobsCOM.Recordset
        Dim oRecordSetMunicipio As SAPbobsCOM.Recordset
        Dim oRecordSetPaisV As SAPbobsCOM.Recordset
        Dim oRecordSetMunicipioV As SAPbobsCOM.Recordset
        Dim oRecordSetTipoCambio As SAPbobsCOM.Recordset
        Dim oRecordSetTipoDocumento As SAPbobsCOM.Recordset
        Dim oRecordSetValidaAjena As SAPbobsCOM.Recordset
        Dim oRecordSetResolucion As SAPbobsCOM.Recordset
        Dim oRecordSetFechaResolucion As SAPbobsCOM.Recordset
        Dim sDecimal As String = ""
        Dim sDecimalQty As String = ""
        Dim Iva As String = ""
        Dim dbandera As Boolean = False
        Dim sCorreo As String
        Dim oRecordSetPago As SAPbobsCOM.Recordset
        Dim oRecordSetNombreComercial As SAPbobsCOM.Recordset
        Dim oRecordSetSociedad As SAPbobsCOM.Recordset
        Dim oRecordSetSerieFace As SAPbobsCOM.Recordset
        Dim lErrCode As Integer = 0
        Dim sErrMsg As String = ""
        Dim oAsientosContables As SAPbobsCOM.JournalEntries
        Dim oRecordSet As SAPbobsCOM.Recordset
        Dim iValida As Integer = 0
        Dim oRecordSetBuscaDatos As SAPbobsCOM.Recordset
        Dim SumaPrecioUnitario As Decimal = 0.0
        Dim SumaPrecio As Decimal = 0.0
        Dim SumaDescuento As Decimal = 0.0
        Dim SumaMontoGravable As Decimal = 0.0
        Dim SumaTotal As Decimal = 0.0
        Dim oRecordSetFace As SAPbobsCOM.Recordset
        Dim TipoPersoneria As String
        Dim oRecordSetPersonaD As SAPbobsCOM.Recordset


        'ADD YOUR ACTION CODE HERE ...

        Try

            oRecordSet = Nothing
            oRecordSet = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
            sQuery = "Select T0.""TaxIdNum"" from OADM T0"
            oRecordSet.DoQuery(sQuery)
            oRecordSet.MoveFirst()

            If oRecordSet.RecordCount = 0 Then
                B1Connections.theAppl.MessageBox("ERROR: NO SE HA DEFINIDO UN NIT")
                Exit Try

            Else

                ' If Validar_Licencia(oRecordSet.Fields.Item(0).Value.ToString()) = False Then
                ' B1Connections.theAppl.MessageBox("ERROR: El NIT No Coincide Con La Licencia")
                ' Exit Try

                ' End If

            End If

            sDocNum = DocNum

            oRecordSetDecimales = Nothing
            oRecordSetDecimales = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
            sQuery = "select ""PriceDec"", ""QtyDec"" from OADM"
            oRecordSetDecimales.DoQuery(sQuery)
            oRecordSetDecimales.MoveFirst()

            If Integer.Parse(oRecordSetDecimales.Fields.Item(0).Value) > 0 Then
                sDecimal = "N" & oRecordSetDecimales.Fields.Item(0).Value
            End If

            If Integer.Parse(oRecordSetDecimales.Fields.Item(1).Value) > 0 Then
                sDecimalQty = "N" & oRecordSetDecimales.Fields.Item(1).Value
            End If


            oRecordSetH = Nothing
            oRecordSetH = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

            If TipoDoc = "13" Then
                sQuery = "Select T1.*,T0.""E_Mail"",T1.""U_FacNit"",T1.""U_NContingencia"", T0.""Phone1"" From OINV T1 inner join OCRD T0 On T0.""CardCode"" = T1.""CardCode"" Where T1.""DocSubType"" <> 'DN' and T1.""DocNum"" = '" & DocNum & "' And T1.""Series"" = " & Series
            ElseIf (TipoDoc = "14") Then
                sQuery = "Select T1.*,T0.""E_Mail"",T1.""U_FacNit"",T1.""U_NContingencia"", T0.""Phone1"" From ORIN T1 inner join OCRD T0 On T0.""CardCode"" = T1.""CardCode"" Where T1.""DocNum"" = '" & DocNum & "' And T1.""Series"" = " & Series
            ElseIf (TipoDoc = "18") Then
                sQuery = "Select T1.*,T0.""E_Mail"",T1.""U_FacNit"",T1.""U_NContingencia"", T0.""Phone1"" From OPCH T1 inner join OCRD T0 On T0.""CardCode"" = T1.""CardCode"" Where T1.""DocNum"" = '" & DocNum & "' And T1.""Series"" = " & Series
            ElseIf (TipoDoc = "DN") Then
                sQuery = "Select T1.*,T0.""E_Mail"",T1.""U_FacNit"",T1.""U_NContingencia"", T0.""Phone1"" From OINV T1 inner join OCRD T0 On T0.""CardCode"" = T1.""CardCode"" Where T1.""DocSubType"" = 'DN' And T1.""DocNum"" = '" & DocNum & "' And T1.""Series"" = " & Series
            ElseIf (TipoDoc = "24") Then
                sQuery = "Select T1.*,T0.""U_Personeria"", T0.""E_Mail"",T0.""Phone1"",T0.""U_NIT"" From ORCT T1 inner join OCRD T0 On T0.""CardCode"" = T1.""CardCode"" Where T1.""DocNum"" = '" & DocNum & "' And T1.""Series"" = " & Series
            End If

            oRecordSetH.DoQuery(sQuery)

            If (TipoDoc = "24") Then
                TipoPersoneria = oRecordSetH.Fields.Item("U_Personeria").Value
            End If


            If Not String.IsNullOrEmpty(oRecordSetH.Fields.Item("CardCode").Value) Then



                oRecordSetSERIESO = Nothing
                oRecordSetSERIESO = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

                If TipoDoc = "24" Then
                    sQuery = "Select * from ""@SERIESO"" where ""U_Serie"" = '" & oRecordSetH.Fields.Item("Series").Value & "' and ""U_Document"" = '" & oRecordSetH.Fields.Item("ObjType").Value & "' "
                Else

                    If oRecordSetH.Fields.Item("DocSubType").Value = "DN" Then
                        sQuery = "Select * from ""@SERIESO"" where ""U_Serie"" = '" & oRecordSetH.Fields.Item("Series").Value & "' and ""U_Document"" = 'DN' "
                    Else
                        sQuery = "Select * from ""@SERIESO"" where ""U_Serie"" = '" & oRecordSetH.Fields.Item("Series").Value & "' and ""U_Document"" = '" & oRecordSetH.Fields.Item("ObjType").Value & "' "
                    End If

                End If



                oRecordSetSERIESO.DoQuery(sQuery)
                oRecordSetSERIESO.MoveFirst()

                If Not String.IsNullOrEmpty(oRecordSetSERIESO.Fields.Item("Object").Value) Then

                    oRecordSetCodigoEmpresa = Nothing
                    oRecordSetCodigoEmpresa = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select ""GlblLocNum"" from ADM1"
                    oRecordSetCodigoEmpresa.DoQuery(sQuery)

                    oRecordSetTipoCambio = Nothing
                    oRecordSetTipoCambio = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select ""Rate"" from ORTT where ""Currency"" = 'USD' and ""RateDate"" = '" & CDate(oRecordSetH.Fields.Item("DocDate").Value).ToString("yyyyMMdd") & "'"
                    oRecordSetTipoCambio.DoQuery(sQuery)
                    oRecordSetTipoCambio.MoveFirst()

                    oRecordSetTaxCodeH = Nothing
                    oRecordSetTaxCodeH = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""TaxCode"" from CRD1 T0 where T0.""CardCode"" = '" & oRecordSetH.Fields.Item("CardCode").Value & "' "
                    sQuery &= " And T0.""AdresType"" = 'S' and T0.""Address"" in (select ""ShipToDef"" from OCRD T1 where T1.""CardCode"" = T0.""CardCode"" ) "
                    oRecordSetTaxCodeH.DoQuery(sQuery)
                    oRecordSetTaxCodeH.MoveFirst()


                    oRecordSetCorreoEmisor = Nothing
                    oRecordSetCorreoEmisor = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""E_Mail"" from OADM T0"
                    oRecordSetCorreoEmisor.DoQuery(sQuery)
                    oRecordSetCorreoEmisor.MoveFirst()


                    oRecordSetNombreComercial = Nothing
                    oRecordSetNombreComercial = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""AliasName"" from OADM T0"
                    oRecordSetNombreComercial.DoQuery(sQuery)
                    oRecordSetNombreComercial.MoveFirst()

                    oRecordSetSociedad = Nothing
                    oRecordSetSociedad = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""PrintHeadr"" from OADM T0"
                    oRecordSetSociedad.DoQuery(sQuery)
                    oRecordSetSociedad.MoveFirst()

                    oRecordSetDireccion = Nothing
                    oRecordSetDireccion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""Street"",T0.""ZipCode"" from ADM1 T0"
                    oRecordSetDireccion.DoQuery(sQuery)
                    oRecordSetDireccion.MoveFirst()

                    oRecordSetDepartamentoV = Nothing
                    oRecordSetDepartamentoV = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T1.""Name"" from OADM T0 inner join OCST T1 On T1.""Code"" = T0.""State"""
                    oRecordSetDepartamentoV.DoQuery(sQuery)
                    oRecordSetDepartamentoV.MoveFirst()

                    oRecordSetMunicipioV = Nothing
                    oRecordSetMunicipioV = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select ""County"" From ADM1 "
                    oRecordSetMunicipioV.DoQuery(sQuery)
                    oRecordSetMunicipioV.MoveFirst()


                    oRecordSetPaisV = Nothing
                    oRecordSetPaisV = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""Country"" From OADM T0 "
                    oRecordSetPaisV.DoQuery(sQuery)
                    oRecordSetPaisV.MoveFirst()


                    oRecordSetCodigoPostal = Nothing
                    oRecordSetCodigoPostal = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""ZipCode"" from CRD1 T0 where T0.""CardCode"" = '" & oRecordSetH.Fields.Item("CardCode").Value & "' "
                    sQuery &= " And T0.""AdresType"" = 'B' and T0.""Address"" in (select ""ShipToDef"" from OCRD T1 where T1.""CardCode"" = T0.""CardCode"" ) "
                    oRecordSetCodigoPostal.DoQuery(sQuery)
                    oRecordSetCodigoPostal.MoveFirst()

                    oRecordSetMunicipio = Nothing
                    oRecordSetMunicipio = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select T0.""County"" from CRD1 T0 where T0.""CardCode"" = '" & oRecordSetH.Fields.Item("CardCode").Value & "' "
                    sQuery &= " And T0.""AdresType"" = 'B' and T0.""Address"" in (select ""ShipToDef"" from OCRD T1 where T1.""CardCode"" = T0.""CardCode"" ) "
                    oRecordSetMunicipio.DoQuery(sQuery)
                    oRecordSetMunicipio.MoveFirst()

                    oRecordSetPais = Nothing
                    oRecordSetPais = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select T0.""Country"" from CRD1 T0 where T0.""CardCode"" = '" & oRecordSetH.Fields.Item("CardCode").Value & "' "
                    sQuery &= " And T0.""AdresType"" = 'B' and T0.""Address"" in (select ""ShipToDef"" from OCRD T1 where T1.""CardCode"" = T0.""CardCode"" ) "
                    oRecordSetPais.DoQuery(sQuery)
                    oRecordSetPais.MoveFirst()

                    oRecordSetDepartamento = Nothing
                    oRecordSetDepartamento = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = " Select T3.""Name"" from OCST T3 where T3.""Code"" In (Select T0.""State"" from CRD1 T0 where T0.""CardCode"" = '" & oRecordSetH.Fields.Item("CardCode").Value & "' "
                    sQuery &= " And T0.""AdresType"" = 'B' and T0.""Address"" in (select ""ShipToDef"" from OCRD T1 where T1.""CardCode"" = T0.""CardCode""))"
                    oRecordSetDepartamento.DoQuery(sQuery)
                    oRecordSetDepartamento.MoveFirst()

                    oRecordSetNitV = Nothing
                    oRecordSetNitV = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select T0.""TaxIdNum"" from OADM T0"
                    oRecordSetNitV.DoQuery(sQuery)
                    oRecordSetNitV.MoveFirst()

                    oRecordSetCodigoEmpresa = Nothing
                    oRecordSetCodigoEmpresa = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select ""GlblLocNum"" from ADM1"
                    oRecordSetCodigoEmpresa.DoQuery(sQuery)

                    oRecordSetCnfgFace = Nothing
                    oRecordSetCnfgFace = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "select * from ""@SERIESO"" T0 Where T0.""U_Serie"" = '" & oRecordSetH.Fields.Item("Series").Value & "'"
                    oRecordSetCnfgFace.DoQuery(sQuery)
                    oRecordSetCnfgFace.MoveFirst()


                    oRecordSetFEL = Nothing
                    oRecordSetFEL = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select * from ""@CONFEL"" "
                    oRecordSetFEL.DoQuery(sQuery)

                    oRegimenISR = oRecordSetFEL.Fields.Item("U_RegISR").Value


                    Dim oRecordSetHDocEntry As SAPbobsCOM.Recordset

                    oRecordSetHDocEntry = Nothing
                    oRecordSetHDocEntry = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

                    If TipoDoc = "12" Or TipoDoc = "13" Or TipoDoc = "exp" Or TipoDoc = "DN" Then
                        oRecordSetHDocEntry = Nothing
                        oRecordSetHDocEntry = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select * from ""OINV"" T0 Where T0.""DocNum"" = " & DocNum
                        oRecordSetHDocEntry.DoQuery(sQuery)
                        oRecordSetHDocEntry.MoveFirst()
                    ElseIf TipoDoc = "14" Or TipoDoc = "21" Then
                        oRecordSetHDocEntry = Nothing
                        oRecordSetHDocEntry = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select * from ""ORIN"" T0 Where T0.""DocNum"" = " & DocNum
                        oRecordSetHDocEntry.DoQuery(sQuery)
                        oRecordSetHDocEntry.MoveFirst()
                    ElseIf TipoDoc = "20" Then
                        oRecordSetHDocEntry = Nothing
                        oRecordSetHDocEntry = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select * from ""OINV"" T0 Where T0.""DocNum"" = " & DocNum
                        oRecordSetHDocEntry.DoQuery(sQuery)
                        oRecordSetHDocEntry.MoveFirst()
                    ElseIf TipoDoc = "18" Then
                        oRecordSetHDocEntry = Nothing
                        oRecordSetHDocEntry = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select * from ""OPCH"" T0 Where T0.""DocNum"" = " & DocNum
                        oRecordSetHDocEntry.DoQuery(sQuery)
                        oRecordSetHDocEntry.MoveFirst()
                    End If

                    sDocEntry = oRecordSetHDocEntry.Fields.Item("DocEntry").Value


                    If oRecordSetFEL.RecordCount = 0 Then
                        B1Connections.theAppl.MessageBox("No Existe Ruta Para Depositar Los Archivos..")
                        Exit Try
                    End If

                    If oRegimenISR = "DIR" Then

                        oRecordSetResolucion = Nothing
                        oRecordSetResolucion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "select  ""U_NumeroR"" from ""@INFORESOLU"""
                        oRecordSetResolucion.DoQuery(sQuery)
                        oRecordSetResolucion.MoveFirst()

                        NumeroResolucion = oRecordSetResolucion.Fields.Item("U_NumeroR").Value

                        oRecordSetFechaResolucion = Nothing
                        oRecordSetFechaResolucion = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "select ""U_FechaR"" from ""@INFORESOLU"""
                        oRecordSetFechaResolucion.DoQuery(sQuery)
                        oRecordSetFechaResolucion.MoveFirst()

                        FechaResolucion = oRecordSetFechaResolucion.Fields.Item("U_FechaR").Value


                    End If



                    If TipoDoc = "13" Then



                        If System.IO.File.Exists(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\Factura_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml") Then
                            'The file exists
                            System.IO.File.Delete(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\Factura_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")

                        End If

                        oArchivo = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\Factura_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")


                    ElseIf TipoDoc = "18" Then



                        If System.IO.File.Exists(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\FacturaEspecial_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml") Then
                            'The file exists
                            System.IO.File.Delete(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\FacturaEspecial_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")

                        End If

                        oArchivo = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\FacturaEspecial_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")


                    ElseIf (TipoDoc = "14") Then


                        If System.IO.File.Exists(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\NotaCredito_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml") Then
                            'The file exists
                            System.IO.File.Delete(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\NotaCredito_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")
                        End If


                        oArchivo = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\NotaCredito_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")

                    ElseIf (TipoDoc = "DN") Then


                        If System.IO.File.Exists(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\NotaDebito_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml") Then
                            'The file exists
                            System.IO.File.Delete(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\NotaDebito_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")

                            'the file doesn't exist

                        End If

                        oArchivo = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\NotaDebito_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")



                    ElseIf (TipoDoc = "24") Then


                        oArchivo = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaXML").Value & "\Recibo_" & oRecordSetH.Fields.Item("DocNum").Value & "_IN1.xml")

                    End If


                    Dim sFechaDoc As String = Convert.ToDateTime(oRecordSetH.Fields.Item("DocDate").Value).ToString("yyyy-MM-ddTHH:mm:ss-06:00")
                    Dim sFechaDoc2 As String = Convert.ToDateTime(Now.Date).ToString("yyyy-MM-ddTHH:mm:ss-06:00")




                    SumaPrecio = 0.0
                    SumaDescuento = 0.0
                    SumaMontoGravable = 0.0
                    SumaTotal = 0.0
                    SumaPrecioUnitario = 0.0


                    SumaIva = 0.0
                    SumaDescuentos = 0.0
                    SumaExento = 0.0
                    SumaNeto = 0.0
                    SumaLineasTotal = 0.0
                    SumaBruto = 0.0
                    SumaDescuentos = 0.0


                    '   oReferencia = oRecordSetCodigoEmpresa.Fields.Item(0).Value

                    ' oReferencia = oReferencia & Strings.Right("111111" & oRecordSetH.Fields.Item("DocEntry").Value, (20 - oReferencia.Length))


                    sXML = "<?xml version=""1.0"" encoding=""utf-8""?>"


                    sXML &= "<dte:GTDocumento xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"" xmlns:cfc=""http://www.sat.gob.gt/dte/fel/CompCambiaria/0.1.0"" xmlns:cno=""http://www.sat.gob.gt/face2/ComplementoReferenciaNota/0.1.0"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:cex=""http://www.sat.gob.gt/face2/ComplementoExportaciones/0.1.0"" xmlns:cfe=""http://www.sat.gob.gt/face2/ComplementoFacturaEspecial/0.1.0"" Version=""0.4"" xmlns:dte = ""http://www.sat.gob.gt/dte/fel/0.1.0"">"

                    sXML &= "  <dte:SAT ClaseDocumento=""dte"">"

                    sXML &= "    <dte:DTE ID = ""DatosCertificados"">"

                    sXML &= "      <dte:DatosEmision ID=""DatosEmision"">"


                    If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                        If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Then
                            sXML &= "        <dte:DatosGenerales CodigoMoneda=""GTQ"" FechaHoraEmision=" & Chr(34) & sFechaDoc & Chr(34) & " Exp=""SI""  Tipo=""FACT""></dte:DatosGenerales>"
                        ElseIf oRecordSetSERIESO.Fields.Item("U_Type").Value = "CAXP" Then
                            sXML &= "        <dte:DatosGenerales CodigoMoneda=""GTQ"" FechaHoraEmision=" & Chr(34) & sFechaDoc & Chr(34) & " Exp=""SI""  Tipo=""FCAM""></dte:DatosGenerales>"
                        ElseIf oRecordSetSERIESO.Fields.Item("U_Type").Value = "RDON" Then
                            sXML &= "        <dte:DatosGenerales CodigoMoneda=""GTQ"" FechaHoraEmision=" & Chr(34) & sFechaDoc & Chr(34) & " Tipo=""RDON"" TipoPersoneria=" & oRecordSetH.Fields.Item("U_Personeria").Value.ToString & "></dte:DatosGenerales>"
                        Else
                            sXML &= "        <dte:DatosGenerales CodigoMoneda=""GTQ"" FechaHoraEmision=" & Chr(34) & sFechaDoc & Chr(34) & " Tipo=" & Chr(34) & oRecordSetSERIESO.Fields.Item("U_Type").Value & Chr(34) & "></dte:DatosGenerales>"
                        End If
                    Else
                        If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Then
                            sXML &= "        <dte:DatosGenerales CodigoMoneda=""USD"" FechaHoraEmision=" & Chr(34) & sFechaDoc & Chr(34) & " Exp=""SI""  Tipo=""FACT""></dte:DatosGenerales>"
                        ElseIf oRecordSetSERIESO.Fields.Item("U_Type").Value = "CAXP" Then
                            sXML &= "        <dte:DatosGenerales CodigoMoneda=""USD"" FechaHoraEmision=" & Chr(34) & sFechaDoc & Chr(34) & " Exp=""SI""  Tipo=""FCAM""></dte:DatosGenerales>"
                        ElseIf oRecordSetSERIESO.Fields.Item("U_Type").Value = "RECI" Then
                            sXML &= "        <dte:DatosGenerales CodigoMoneda=""GTQ"" FechaHoraEmision=" & Chr(34) & sFechaDoc & Chr(34) & " Tipo=""RDON"" TipoPersoneria=" & Chr(34) & oRecordSetH.Fields.Item("U_Personeria").Value.ToString & Chr(34) & "></dte:DatosGenerales>"
                        Else
                            sXML &= "        <dte:DatosGenerales CodigoMoneda=""USD"" FechaHoraEmision=" & Chr(34) & sFechaDoc & Chr(34) & " Tipo=" & Chr(34) & oRecordSetSERIESO.Fields.Item("U_Type").Value & Chr(34) & "></dte:DatosGenerales>"
                        End If
                    End If

                    If oRecordSetCnfgFace.RecordCount > 0 Then
                        If oRecordSetFEL.Fields.Item("U_IVA").Value = "EXE" Then
                            sXML &= "        <dte:Emisor AfiliacionIVA=" & Chr(34) & "GEN" & Chr(34) & " CodigoEstablecimiento=" & Chr(34) & oRecordSetCnfgFace.Fields.Item("U_Codigo").Value & Chr(34) & " CorreoEmisor=" & Chr(34) & oRecordSetCorreoEmisor.Fields.Item(0).Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " NITEmisor=" & Chr(34) & oRecordSetNitV.Fields.Item(0).Value.ToString().Replace("-", "") & Chr(34) & " NombreComercial=" & Chr(34) & oRecordSetCnfgFace.Fields.Item("U_NombreEs").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " NombreEmisor=" & Chr(34) & oRecordSetSociedad.Fields.Item(0).Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & ">"
                        Else
                            sXML &= "        <dte:Emisor AfiliacionIVA=" & Chr(34) & oRecordSetFEL.Fields.Item("U_IVA").Value & Chr(34) & " CodigoEstablecimiento=" & Chr(34) & oRecordSetCnfgFace.Fields.Item("U_Codigo").Value & Chr(34) & " CorreoEmisor=" & Chr(34) & oRecordSetCorreoEmisor.Fields.Item(0).Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " NITEmisor=" & Chr(34) & oRecordSetNitV.Fields.Item(0).Value.ToString().Replace("-", "") & Chr(34) & " NombreComercial=" & Chr(34) & oRecordSetCnfgFace.Fields.Item("U_NombreEs").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " NombreEmisor=" & Chr(34) & oRecordSetSociedad.Fields.Item(0).Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & ">"
                        End If
                    Else
                        B1Connections.theAppl.MessageBox("Esta Serie No Es Electronica")
                        Exit Try
                    End If

                    sXML &= "          <dte:DireccionEmisor>"

                    sXML &= "            <dte:Direccion>" & oRecordSetCnfgFace.Fields.Item("U_Direcc").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:Direccion>"

                    sXML &= "            <dte:CodigoPostal>" & oRecordSetCnfgFace.Fields.Item("U_Postal").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:CodigoPostal>"

                    sXML &= "            <dte:Municipio>" & oRecordSetCnfgFace.Fields.Item("U_Muni").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:Municipio>"

                    sXML &= "            <dte:Departamento>" & oRecordSetCnfgFace.Fields.Item("U_Depart").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:Departamento>"

                    sXML &= "            <dte:Pais>" & oRecordSetPaisV.Fields.Item(0).Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:Pais>"

                    sXML &= "          </dte:DireccionEmisor>"

                    sXML &= "        </dte:Emisor>"



                    If Not String.IsNullOrEmpty(oRecordSetH.Fields.Item("E_Mail").Value) Then
                        sCorreo = oRecordSetH.Fields.Item("E_Mail").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;")
                        sCorreo = sCorreo.Trim().TrimEnd().TrimStart().Replace(" ", "")


                        If String.IsNullOrEmpty(oRecordSetH.Fields.Item("U_DocNom").Value.ToString()) Then

                            If oRecordSetH.Fields.Item("U_FacNit").Value.ToString().Replace("-", "").Trim().TrimEnd().TrimStart().Length = 13 Then

                                If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Then
                                    sXML &= "        <dte:Receptor IDReceptor=""CF"" TipoEspecial= ""CUI"" NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("CardName").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & sCorreo.Replace(" ", "").ToString().TrimEnd().TrimStart().Trim().Replace("/", ";").Replace(",", ";").Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & ";" & oRecordSetFEL.Fields.Item("U_Correo").Value & Chr(34) & ">"
                                Else
                                    sXML &= "        <dte:Receptor IDReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_FacNit").Value.ToString().Replace("-", "") & Chr(34) & " TipoEspecial= ""CUI"" NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("CardName").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & sCorreo.Replace(" ", "").ToString().TrimEnd().TrimStart().Trim().Replace("/", ";").Replace(",", ";").Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & ";" & oRecordSetFEL.Fields.Item("U_Correo").Value & Chr(34) & ">"
                                End If

                            Else

                                If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Then
                                    sXML &= "        <dte:Receptor IDReceptor=""CF"" NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("CardName").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & sCorreo.Replace(" ", "").ToString().TrimEnd().TrimStart().Trim().Replace("/", ";").Replace(",", ";").Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & ";" & oRecordSetFEL.Fields.Item("U_Correo").Value & Chr(34) & ">"
                                Else
                                    sXML &= "        <dte:Receptor IDReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_FacNit").Value.ToString().Replace("-", "") & Chr(34) & " NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("CardName").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & sCorreo.Replace(" ", "").ToString().TrimEnd().TrimStart().Trim().Replace("/", ";").Replace(",", ";").Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & ";" & oRecordSetFEL.Fields.Item("U_Correo").Value & Chr(34) & ">"
                                End If

                            End If
                        Else

                            If oRecordSetH.Fields.Item("U_FacNit").Value.ToString().Replace("-", "").Trim().TrimEnd().TrimStart().Length = 13 Then

                                If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Then
                                    sXML &= "        <dte:Receptor IDReceptor=""CF"" TipoEspecial= ""CUI"" NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_DocNom").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & sCorreo.Replace(" ", "").ToString().TrimEnd().TrimStart().Trim().Replace("/", ";").Replace(",", ";").Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & ";" & oRecordSetFEL.Fields.Item("U_Correo").Value & Chr(34) & ">"
                                Else
                                    sXML &= "        <dte:Receptor IDReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_FacNit").Value.ToString().Replace("-", "") & Chr(34) & " TipoEspecial= ""CUI"" NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_DocNom").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & sCorreo.Replace(" ", "").ToString().TrimEnd().TrimStart().Trim().Replace("/", ";").Replace(",", ";").Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & ";" & oRecordSetFEL.Fields.Item("U_Correo").Value & Chr(34) & ">"
                                End If

                            Else

                                If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Then
                                    sXML &= "        <dte:Receptor IDReceptor=""CF"" NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_DocNom").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & sCorreo.Replace(" ", "").ToString().TrimEnd().TrimStart().Trim().Replace("/", ";").Replace(",", ";").Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & ";" & oRecordSetFEL.Fields.Item("U_Correo").Value & Chr(34) & ">"
                                Else
                                    sXML &= "        <dte:Receptor IDReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_FacNit").Value.ToString().Replace("-", "") & Chr(34) & " NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_DocNom").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & sCorreo.Replace(" ", "").ToString().TrimEnd().TrimStart().Trim().Replace("/", ";").Replace(",", ";").Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & ";" & oRecordSetFEL.Fields.Item("U_Correo").Value & Chr(34) & ">"
                                End If

                            End If

                        End If

                    Else

                        If String.IsNullOrEmpty(oRecordSetH.Fields.Item("U_DocNom").Value.ToString()) Then

                            If oRecordSetH.Fields.Item("U_FacNit").Value.ToString().Replace("-", "").Trim().TrimEnd().TrimStart().Length = 13 Then

                                If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Then
                                    sXML &= "        <dte:Receptor IDReceptor=""CF"" TipoEspecial= ""CUI"" NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("CardName").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & Chr(34) & ">"
                                Else
                                    sXML &= "        <dte:Receptor IDReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_FacNit").Value.ToString().Replace("-", "") & Chr(34) & " TipoEspecial= ""CUI"" NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("CardName").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & Chr(34) & ">"
                                End If

                            Else

                                If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Then
                                    sXML &= "        <dte:Receptor IDReceptor=""CF"" NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("CardName").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & Chr(34) & ">"
                                Else
                                    sXML &= "        <dte:Receptor IDReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_FacNit").Value.ToString().Replace("-", "") & Chr(34) & " NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("CardName").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & Chr(34) & ">"
                                End If

                            End If

                        Else

                            If oRecordSetH.Fields.Item("U_FacNit").Value.ToString().Replace("-", "").Trim().TrimEnd().TrimStart().Length = 13 Then
                                If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Then
                                    sXML &= "        <dte:Receptor IDReceptor=""CF"" TipoEspecial= ""CUI"" NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_DocNom").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & Chr(34) & ">"
                                Else
                                    sXML &= "        <dte:Receptor IDReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_FacNit").Value.ToString().Replace("-", "") & Chr(34) & " TipoEspecial= ""CUI"" NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_DocNom").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & Chr(34) & ">"
                                End If

                            Else

                                If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Then
                                    sXML &= "        <dte:Receptor IDReceptor=""CF"" NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_DocNom").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & Chr(34) & ">"
                                Else
                                    sXML &= "        <dte:Receptor IDReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_FacNit").Value.ToString().Replace("-", "") & Chr(34) & " NombreReceptor=" & Chr(34) & oRecordSetH.Fields.Item("U_DocNom").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " CorreoReceptor=" & Chr(34) & Chr(34) & ">"
                                End If

                            End If
                        End If
                    End If

                    sXML &= "          <dte:DireccionReceptor>"

                    If oRecordSetH.Fields.Item("Address").Value = "" Then
                        sXML &= "            <dte:Direccion>CIUDAD</dte:Direccion>"
                    Else
                        sXML &= "            <dte:Direccion>" & quitarSaltosLinea(oRecordSetH.Fields.Item("Address").Value, " ").ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:Direccion>"
                    End If

                    If Not String.IsNullOrEmpty(oRecordSetCodigoPostal.Fields.Item(0).Value.ToString()) Then
                        sXML &= "            <dte:CodigoPostal>" & oRecordSetCodigoPostal.Fields.Item(0).Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:CodigoPostal>"
                    Else
                        sXML &= "            <dte:CodigoPostal>01001</dte:CodigoPostal>"
                    End If

                    If Not String.IsNullOrEmpty(oRecordSetMunicipio.Fields.Item(0).Value.ToString()) Then
                        sXML &= "            <dte:Municipio>" & oRecordSetMunicipio.Fields.Item(0).Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:Municipio>"
                    Else
                        sXML &= "            <dte:Municipio>Guatemala</dte:Municipio>"
                    End If


                    If Not String.IsNullOrEmpty(oRecordSetDepartamento.Fields.Item(0).Value.ToString()) Then
                        sXML &= "            <dte:Departamento>" & oRecordSetDepartamento.Fields.Item(0).Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:Departamento>"
                    Else
                        sXML &= "            <dte:Departamento>Guatemala</dte:Departamento>"
                    End If

                    If Not String.IsNullOrEmpty(oRecordSetPais.Fields.Item(0).Value.ToString()) Then
                        sXML &= "            <dte:Pais>" & oRecordSetPais.Fields.Item(0).Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:Pais>"
                    Else
                        sXML &= "            <dte:Pais>GT</dte:Pais>"
                    End If

                    sXML &= "          </dte:DireccionReceptor>"

                    sXML &= "        </dte:Receptor>"

                    If Not oRecordSetSERIESO.Fields.Item("U_Type").Value = "FESP" Or oRecordSetSERIESO.Fields.Item("U_Type").Value = "NABN" Then

                        If oRecordSetSERIESO.Fields.Item("U_Type").Value = "FACT" Or oRecordSetSERIESO.Fields.Item("U_Type").Value = "FCAM" Or oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Or oRecordSetSERIESO.Fields.Item("U_Type").Value = "CAXP" Then

                            sXML &= "        <dte:Frases>"
                            If oRegimenISR = "RET" Then
                                sXML &= "          <dte:Frase TipoFrase=""1"" CodigoEscenario=""2"" />"
                            ElseIf oRegimenISR = "DIR" Then
                                sXML &= "          <dte:Frase TipoFrase=""1"" CodigoEscenario=""3"" NumeroResolucion=""" & NumeroResolucion.Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & """ FechaResolucion=""" & FechaResolucion.ToString("yyyy-MM-dd") & """ />"
                            ElseIf oRegimenISR = "EXE" Then
                                sXML &= "          <dte:Frase TipoFrase=""1"" CodigoEscenario=""4"" />"
                            Else
                                sXML &= "          <dte:Frase TipoFrase=""1"" CodigoEscenario=""1"" />"
                            End If

                            If oRecordSetFEL.Fields.Item("U_Agente").Value = "Y" Then
                                sXML &= "          <dte:Frase TipoFrase=""2"" CodigoEscenario=""1"" />"
                            End If

                            If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Or Not String.IsNullOrEmpty(oRecordSetH.Fields.Item("U_Frases").Value) Then

                                If String.IsNullOrEmpty(oRecordSetH.Fields.Item("U_Frases").Value) Then
                                    B1Connections.theAppl.MessageBox("Alerta: Seleccione Una Frase")
                                    oArchivo.Close()
                                    oArchivo.Flush()
                                    Exit Try
                                Else

                                    If Integer.Parse(oRecordSetH.Fields.Item("U_Frases").Value.ToString.Length) = 3 Then
                                        sXML &= "          <dte:Frase TipoFrase=" & Chr(34) & oRecordSetH.Fields.Item("U_Frases").Value.ToString().Substring(0, 1) & Chr(34) & " CodigoEscenario=" & Chr(34) & oRecordSetH.Fields.Item("U_Frases").Value.ToString().Substring(2, 1) & Chr(34) & " />"
                                    Else
                                        sXML &= "          <dte:Frase TipoFrase=" & Chr(34) & oRecordSetH.Fields.Item("U_Frases").Value.ToString().Substring(0, 1) & Chr(34) & " CodigoEscenario=" & Chr(34) & oRecordSetH.Fields.Item("U_Frases").Value.ToString().Substring(2, 2) & Chr(34) & " />"
                                    End If

                                End If

                            End If
                            sXML &= "        </dte:Frases>"

                        End If
                    End If

                    oRecordSetAgrupa = Nothing
                    oRecordSetAgrupa = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

                    If TipoDoc = "13" Then
                        sQuery = "Select T0.""U_Agrupa"" from OINV T0 where T0.""DocEntry"" = '" & oRecordSetH.Fields.Item("DocEntry").Value & "'"
                    ElseIf (TipoDoc = "14") Then
                        sQuery = "Select T0.""U_Agrupa"" from ORIN T0 where T0.""DocEntry"" = '" & oRecordSetH.Fields.Item("DocEntry").Value & "'"
                    ElseIf (TipoDoc = "DN") Then
                        sQuery = "Select T0.""U_Agrupa"" from OINV T0 where T0.""DocEntry"" = '" & oRecordSetH.Fields.Item("DocEntry").Value & "'"
                    End If

                    oRecordSetAgrupa.DoQuery(sQuery)

                    If oRecordSetAgrupa.Fields.Item("U_Agrupa").Value = "Yes" Or oRecordSetAgrupa.Fields.Item("U_Agrupa").Value = "Y" Then
                        AgrupaDetalle = "SI"
                    Else
                        AgrupaDetalle = "NO"
                    End If


                    oRecordSetD = Nothing
                    oRecordSetD = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

                    If TipoDoc = "13" Then

                        sQuery = "Select *,(CASE When T0.""VatSum"" = 0.00 Then T0.""GTotal"" Else 0.00 End) ""ImporteExento"" , "
                        sQuery &= "(CASE When T0.""TaxCode"" = 'IVA' Then T0.""VatSum"" Else 0.00 End) ""IVA"" , "

                        sQuery &= "T0.""DiscPrcnt"" ""MontoDescuento"" , "
                        sQuery &= "T0.""Quantity"" ""CantidadLinea"" , "

                        sQuery &= "T0.""PriceAfVAT"" ""PrecioUnitario"" , "
                        sQuery &= "(CASE When T0.""VatSum"" = 0.00 Then 0.00 Else T0.""GTotal"" End) ""ImporteNetoGravado"", (SELECT  ""LineText"" From INV10 Where T0.""VisOrder"" =  INV10.""LineSeq"" and INV10.""DocEntry"" = T0.""DocEntry"") ""LineText"", T0.""FreeTxt"" From INV1 T0 Where T0.""DocEntry"" = " & oRecordSetH.Fields.Item("DocEntry").Value

                    ElseIf TipoDoc = "18" Then

                        'MsgBox("Entro")
                        sQuery = "Select *,(CASE When T0.""VatSum"" = 0.00 Then T0.""GTotal"" Else 0.00 End) ""ImporteExento"" , "
                        sQuery &= "(CASE When T0.""TaxCode"" = 'IVA' Then T0.""VatSum"" Else 0.00 End) ""IVA"" , "


                        sQuery &= "T0.""DiscPrcnt"" ""MontoDescuento"" , "
                        sQuery &= "T0.""Quantity"" ""CantidadLinea"" , "

                        sQuery &= "T0.""PriceAfVAT"" ""PrecioUnitario"" , "

                        sQuery &= "(CASE When T0.""VatSum"" = 0.00 Then 0.00 Else T0.""GTotal"" End) ""ImporteNetoGravado"", (SELECT  ""LineText"" From INV10 Where T0.""VisOrder"" =  INV10.""LineSeq"" and INV10.""DocEntry"" = T0.""DocEntry"") ""LineText"", T0.""FreeTxt"" From PCH1 T0 Where T0.""DocEntry"" = " & oRecordSetH.Fields.Item("DocEntry").Value

                    ElseIf (TipoDoc = "14") Then

                        sQuery = "Select *,(Case When T0.""VatSum"" = 0.00 Then T0.""GTotal"" Else 0.00 End) ""ImporteExento"" , "
                        sQuery &= "(CASE When T0.""TaxCode"" = 'IVA' Then T0.""VatSum"" Else 0.00 End) ""IVA"" , "

                        sQuery &= "T0.""DiscPrcnt"" ""MontoDescuento"" , "
                        sQuery &= "T0.""Quantity"" ""CantidadLinea"" , "

                        sQuery &= "T0.""PriceAfVAT"" ""PrecioUnitario"" , "

                        sQuery &= "(CASE When T0.""VatSum"" = 0.00 Then 0.00 Else T0.""GTotal"" End)  ""ImporteNetoGravado"", (SELECT  ""LineText"" From INV10 Where T0.""VisOrder"" =  INV10.""LineSeq"" and INV10.""DocEntry"" = T0.""DocEntry"") ""LineText"", T0.""FreeTxt"" From RIN1 T0 Where T0.""DocEntry"" = " & oRecordSetH.Fields.Item("DocEntry").Value

                    ElseIf (TipoDoc = "DN") Then

                        sQuery = "Select *,(CASE When T0.""VatSum"" = 0.00 Then T0.""GTotal"" Else 0.00 End) ""ImporteExento"" , "
                        sQuery &= "(CASE When T0.""TaxCode"" = 'IVA' Then T0.""VatSum"" Else 0.00 End) ""IVA"" , "

                        sQuery &= "T0.""DiscPrcnt"" ""MontoDescuento"" , "
                        sQuery &= "T0.""Quantity"" ""CantidadLinea"" , "

                        sQuery &= "T0.""PriceAfVAT"" ""PrecioUnitario"" , "

                        sQuery &= "(CASE When T0.""VatSum"" = 0.00 Then 0.00 Else T0.""GTotal"" End) ""ImporteNetoGravado"", (SELECT  ""LineText"" From INV10 Where T0.""VisOrder"" =  INV10.""LineSeq"" and INV10.""DocEntry"" = T0.""DocEntry"") ""LineText"", T0.""FreeTxt"" From INV1 T0 Where T0.""DocEntry"" = " & oRecordSetH.Fields.Item("DocEntry").Value

                    ElseIf (TipoDoc = "24") Then
                        sQuery = "Select * From RCT2 T0 Where T0.""DocNum"" = " & oRecordSetH.Fields.Item("DocNum").Value
                    End If




                    Dim oBanderaH As Boolean = False

                    If oBanderaH = False Then
                        oBanderaH = False
                    Else

                        If oBanderaH = False Then
                            oBanderaH = False
                        Else
                            oBanderaH = True
                        End If

                    End If


                    oRecordSetD.DoQuery(sQuery)

                    'PROCESO DE ALMACENAMIENTO EN DESCUENTO PARA EVITAR DESCUENTOS A 100% 
                    'CALCULO DE PRECIOS

                    PrecioLinea = Math.Round(oRecordSetD.Fields.Item("PrecioUnitario").Value, 2)
                    CantidadLinea = Math.Round(oRecordSetD.Fields.Item("CantidadLinea").Value, 2)
                    DescuentoLinea = Math.Round(oRecordSetD.Fields.Item("MontoDescuento").Value, 2)

                    If oRecordSetH.Fields.Item("DocType").Value = "I" Then
                        DescuentoTotalLinea = Math.Round((((100 * PrecioLinea / (100 - DescuentoLinea)) - PrecioLinea) * CantidadLinea), 2)
                    ElseIf oRecordSetH.Fields.Item("DocType").Value = "S" Then
                        DescuentoTotalLinea = Math.Round(((100 * PrecioLinea / (100 - DescuentoLinea)) - PrecioLinea), 2)
                    End If


                    '********** FIN DESCUENTOS **************

                    Dim iLineaContador As Integer = 1

                    sXML &= "        <dte:Items>"

                    'VALIDA LA VARIABLE AGRUPADETALLE PARA ASIGNAR LA LINEA DE DETALLE
                    If AgrupaDetalle = "SI" Then

                        Subtotal1 = Math.Round(Convert.ToDouble(oRecordSetH.Fields.Item("U_SubTotal1").Value.ToString()), 2)
                        Subtotal2 = Math.Round(Convert.ToDouble(oRecordSetH.Fields.Item("U_SubTotal2").Value.ToString()), 2)

                        TotalAgrupado = Math.Round((Subtotal1 + Subtotal2), 2)

                        GravableAgrupado = Math.Round((TotalAgrupado / 1.12), 2)
                        ImpuestoAgrupado = Math.Round((GravableAgrupado * 0.12), 2)

                        'Primera Linea
                        sXML &= "          <dte:Item NumeroLinea=" & Chr(34) & "1" & Chr(34) & " BienOServicio=""S"">"
                        sXML &= "            <dte:Cantidad>1</dte:Cantidad>"
                        sXML &= "            <dte:UnidadMedida>UND</dte:UnidadMedida>"
                        sXML &= "            <dte:Descripcion>" & oRecordSetH.Fields.Item("U_Detfact").Value.ToString() & "</dte:Descripcion>"
                        sXML &= "            <dte:PrecioUnitario>" & Subtotal1 & "</dte:PrecioUnitario>"
                        sXML &= "            <dte:Precio>" & Subtotal1 & "</dte:Precio>"
                        sXML &= "            <dte:Descuento>0.00</dte:Descuento>"

                        sXML &= "            <dte:Impuestos>"
                        sXML &= "              <dte:Impuesto>"
                        sXML &= "                <dte:NombreCorto>IVA</dte:NombreCorto>"
                        sXML &= "                <dte:CodigoUnidadGravable>1</dte:CodigoUnidadGravable>"
                        sXML &= "                <dte:MontoGravable>" & Math.Round((Subtotal1 / 1.12), 2) & "</dte:MontoGravable>"
                        sXML &= "                <dte:MontoImpuesto>" & Math.Round(((Subtotal1 / 1.12) * 0.12), 2) & "</dte:MontoImpuesto>"
                        sXML &= "              </dte:Impuesto>"
                        sXML &= "            </dte:Impuestos>"

                        sXML &= "            <dte:Total>" & Subtotal1 & "</dte:Total>"
                        sXML &= "          </dte:Item>"

                        'Segunda Linea
                        sXML &= "          <dte:Item NumeroLinea=" & Chr(34) & "2" & Chr(34) & " BienOServicio=""S"">"
                        sXML &= "            <dte:Cantidad>1</dte:Cantidad>"
                        sXML &= "            <dte:UnidadMedida>UND</dte:UnidadMedida>"
                        sXML &= "            <dte:Descripcion>" & oRecordSetH.Fields.Item("U_Detfact2").Value.ToString() & "</dte:Descripcion>"
                        sXML &= "            <dte:PrecioUnitario>" & Subtotal2 & "</dte:PrecioUnitario>"
                        sXML &= "            <dte:Precio>" & Subtotal2 & "</dte:Precio>"
                        sXML &= "            <dte:Descuento>0.00</dte:Descuento>"

                        sXML &= "            <dte:Impuestos>"
                        sXML &= "              <dte:Impuesto>"
                        sXML &= "                <dte:NombreCorto>IVA</dte:NombreCorto>"
                        sXML &= "                <dte:CodigoUnidadGravable>1</dte:CodigoUnidadGravable>"
                        sXML &= "                <dte:MontoGravable>" & Math.Round((Subtotal2 / 1.12), 2) & "</dte:MontoGravable>"
                        sXML &= "                <dte:MontoImpuesto>" & Math.Round(((Subtotal2 / 1.12) * 0.12), 2) & "</dte:MontoImpuesto>"
                        sXML &= "              </dte:Impuesto>"
                        sXML &= "            </dte:Impuestos>"

                        sXML &= "            <dte:Total>" & Subtotal2 & "</dte:Total>"
                        sXML &= "          </dte:Item>"


                        'FIN DE LINEAS
                    Else

                        For iCont As Integer = 0 To oRecordSetD.RecordCount - 1

                            'Valida Kit's o Lista de materiales
                            oRecordSetTipoLinea = Nothing
                            oRecordSetTipoLinea = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)


                            If TipoDoc = "13" Then

                                sQuery = "select T1.""TreeType"",T1.""PriceAfVAT""  from OINV T0 inner join INV1 T1 on T0.""DocEntry"" = T1.""DocEntry""  "

                            ElseIf (TipoDoc = "14") Then


                                sQuery = "select T1.""TreeType"",T1.""PriceAfVAT"" from ORIN T0 inner join RIN1 T1 on T0.""DocEntry"" = T1.""DocEntry""  "

                            ElseIf (TipoDoc = "18") Then


                                sQuery = "select T1.""TreeType"",T1.""PriceAfVAT"" from OPCH T0 inner join PCH1 T1 on T0.""DocEntry"" = T1.""DocEntry""  "

                            ElseIf (TipoDoc = "DN") Then

                                sQuery = "select T1.""TreeType"",T1.""PriceAfVAT"" from OINV T0 inner join INV1  T1 on T0.""DocEntry"" = T1.""DocEntry""  "

                            End If

                            If oRecordSetH.Fields.Item("DocType").Value = "I" Then
                                sQuery &= "where T0.""Series"" = " & Series & " AND T0.""DocNum"" = " & oRecordSetH.Fields.Item("DocNum").Value & " and T1.""ItemCode"" = '" & oRecordSetD.Fields.Item("ItemCode").Value & "' and T1.""VisOrder"" = " & iCont
                            Else
                                sQuery &= "where T0.""Series"" = " & Series & " AND T0.""DocNum"" = " & oRecordSetH.Fields.Item("DocNum").Value & " and T1.""VisOrder"" = " & iCont
                            End If

                            oRecordSetTipoLinea.DoQuery(sQuery)
                            oRecordSetTipoLinea.MoveFirst()

                            If oRecordSetTipoLinea.Fields.Item(0).Value <> "I" Then  'And Double.Parse(oRecordSetTipoLinea.Fields.Item(1).Value) <> 0

                                Dim total As Decimal = 0.0
                                Dim nOp1 As Double = 0.0
                                Dim nOp2 As Double = 0.0
                                Dim nOp3 As Double = 0.0

                                If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                    total = Convert.ToDecimal(oRecordSetD.Fields.Item("GTotal").Value).ToString(sDecimal).Replace(",", "")
                                Else
                                    total = Convert.ToDecimal(oRecordSetD.Fields.Item("GTotalFC").Value).ToString(sDecimal).Replace(",", "")
                                End If

                                Dim num10 As Double = Math.Round(Math.Round(Convert.ToDouble(total) / 1.12, 8), 8)
                                Dim num11 As Double = Math.Round(Math.Round(num10 * 0.12, 6), 4)

                                If oRecordSetSERIESO.Fields.Item("U_Type").Value = "FESP" Then

                                    nOp3 = Convert.ToDecimal(oRecordSetD.Fields.Item("PrecioUnitario").Value).ToString(sDecimal).Replace(",", "")
                                Else
                                    nOp1 = 1.0 - Convert.ToDecimal(oRecordSetD.Fields.Item("DiscPrcnt").Value).ToString(sDecimal).Replace(",", "") / 100.0
                                    nOp2 = Convert.ToDecimal(oRecordSetD.Fields.Item("PrecioUnitario").Value).ToString(sDecimal).Replace(",", "")

                                    Dim i As Integer
                                    Dim cad(30) As String
                                    Dim resultado As String = ""
                                    Dim nSI As Boolean = False

                                    For i = 1 To Len(nOp2.ToString)

                                        If Mid(nOp2.ToString, i, 1) = "." Then
                                            nSI = True
                                        End If

                                        If nSI = True Then
                                            resultado &= Mid(nOp2.ToString, i, 1)
                                        End If
                                    Next i
                                    If nOp1 > 0 Then
                                        nOp3 = Math.Round(Math.Round(nOp2 / nOp1, Integer.Parse(resultado.Replace(".", "").Length)), Integer.Parse(resultado.Replace(".", "").Length))
                                    Else
                                        nOp3 = 0.00
                                    End If
                                End If

                                If oBanderaH = False Then

                                    If oRecordSetH.Fields.Item("DocType").Value = "I" Then

                                        If oRecordSetFEL.Fields.Item("U_Combo").Value = "Y" Then
                                            If oRecordSetD.Fields.Item("TreeType").Value = "I" Then
                                                oRecordSetD.MoveNext()
                                            End If
                                        End If

                                        'VALIDA Y OBTIENE LINEA DE CUENTA AJENA

                                        If oRecordSetFEL.Fields.Item("U_CobroAje").Value = "Y" Then

                                            ProcesaCuentaAjena = ""
                                            LineaCuentaAjena = Nothing



                                            oRecordSetValidaAjena = Nothing
                                            oRecordSetValidaAjena = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                                            sQuery = "Select T1.""U_tipo_articuloCA"" from OITM T1 where T1.""ItemCode"" = '" & oRecordSetD.Fields.Item("ItemCode").Value & "' "
                                            oRecordSetValidaAjena.DoQuery(sQuery)

                                            If oRecordSetValidaAjena.Fields.Item("U_tipo_articuloCA").Value = "02" Then
                                                LineaCuentaAjena = oRecordSetD.Fields.Item("VisOrder").Value
                                                ProcesaCuentaAjena = "Y"
                                            Else
                                                LineaCuentaAjena = Nothing
                                                ProcesaCuentaAjena = "N"
                                            End If


                                        End If

                                        oRecordSetTipoDocumento = Nothing
                                        oRecordSetTipoDocumento = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                                        sQuery = "Select T1.""U_TipoArticuloBS"" from OITM T1 where T1.""ItemCode"" = '" & oRecordSetD.Fields.Item("ItemCode").Value & "' "
                                        oRecordSetTipoDocumento.DoQuery(sQuery)

                                        If String.IsNullOrEmpty(oRecordSetTipoDocumento.Fields.Item(0).Value) Then
                                            sXML &= "          <dte:Item NumeroLinea=" & Chr(34) & iLineaContador & Chr(34) & " BienOServicio="""">"

                                        ElseIf oRecordSetTipoDocumento.Fields.Item(0).Value = "BB" Then
                                            sXML &= "          <dte:Item NumeroLinea=" & Chr(34) & iLineaContador & Chr(34) & " BienOServicio=""B"">"

                                        ElseIf oRecordSetTipoDocumento.Fields.Item(0).Value = "S" Then
                                            sXML &= "          <dte:Item NumeroLinea=" & Chr(34) & iLineaContador & Chr(34) & " BienOServicio=""S"">"

                                        End If

                                    ElseIf oRecordSetH.Fields.Item("DocType").Value = "S" Then

                                        sXML &= "          <dte:Item NumeroLinea=" & Chr(34) & iLineaContador & Chr(34) & " BienOServicio=""S"">"

                                    End If

                                    If oRecordSetH.Fields.Item("DocType").Value = "I" Then
                                        sXML &= "            <dte:Cantidad>" & Convert.ToDecimal(oRecordSetD.Fields.Item("Quantity").Value).ToString(sDecimalQty).Replace(",", "") & "</dte:Cantidad>"

                                    ElseIf oRecordSetH.Fields.Item("DocType").Value = "S" Then
                                        sXML &= "            <dte:Cantidad>1</dte:Cantidad>"

                                    End If


                                    If String.IsNullOrEmpty(oRecordSetD.Fields.Item("unitMsr").Value.ToString()) Then
                                        If String.IsNullOrEmpty(oRecordSetD.Fields.Item("UomCode").Value.ToString()) Then
                                            sXML &= "            <dte:UnidadMedida>UND</dte:UnidadMedida>"
                                        Else
                                            sXML &= "            <dte:UnidadMedida>" & oRecordSetD.Fields.Item("UomCode").Value.ToString().Trim().Substring(0, 3) & "</dte:UnidadMedida>"
                                        End If
                                    Else
                                        If Len(oRecordSetD.Fields.Item("unitMsr").Value) < 3 Then
                                            sXML &= "            <dte:UnidadMedida>UNI</dte:UnidadMedida>"
                                        Else
                                            sXML &= "            <dte:UnidadMedida>" & oRecordSetD.Fields.Item("unitMsr").Value.ToString().Trim().Substring(0, 3) & "</dte:UnidadMedida>"

                                        End If
                                    End If

                                    If Query_Manager_D("dte:Descripcion", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                        If oRecordSetFEL.Fields.Item("U_Free").Value = "Y" Then
                                            sXML &= "            <dte:Descripcion>" & oRecordSetD.Fields.Item("ItemCode").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & " | " & oRecordSetD.Fields.Item("Dscription").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & " | " & oRecordSetD.Fields.Item("U_TextoLargo").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "  " & oRecordSetD.Fields.Item("LineText").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:Descripcion>"
                                        Else
                                            sXML &= "            <dte:Descripcion>" & oRecordSetD.Fields.Item("ItemCode").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & " | " & oRecordSetD.Fields.Item("Dscription").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:Descripcion>"
                                        End If

                                    Else
                                        sXML &= "            <dte:Descripcion>" & Query_Manager_D("dte:Descripcion", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:Descripcion>"
                                    End If

                                    sXML &= "            <dte:PrecioUnitario>" & Convert.ToDecimal(nOp3).ToString(sDecimal).Replace(",", "") & "</dte:PrecioUnitario>"

                                    If oRecordSetH.Fields.Item("DocType").Value = "I" Then
                                        sXML &= "            <dte:Precio>" & Convert.ToDecimal(nOp3 * oRecordSetD.Fields.Item("Quantity").Value).ToString(sDecimal).Replace(",", "") & "</dte:Precio>"

                                    ElseIf oRecordSetH.Fields.Item("DocType").Value = "S" Then
                                        sXML &= "            <dte:Precio>" & Convert.ToDecimal(nOp3 * 1).ToString(sDecimal).Replace(",", "") & "</dte:Precio>"

                                    End If

                                    If oRecordSetH.Fields.Item("DocType").Value = "I" Then

                                        If Convert.ToDecimal(oRecordSetD.Fields.Item("MontoDescuento").Value).ToString(sDecimal).Replace(",", "") > 0 Then
                                            sXML &= "            <dte:Descuento>" & Convert.ToDecimal((nOp3 - nOp2) * Convert.ToDecimal(oRecordSetD.Fields.Item("Quantity").Value).ToString(sDecimal).Replace(",", "")).ToString(sDecimal).Replace(",", "") & "</dte:Descuento>"
                                        Else
                                            sXML &= "            <dte:Descuento>0</dte:Descuento>"
                                        End If

                                    ElseIf oRecordSetH.Fields.Item("DocType").Value = "S" Then

                                        If Convert.ToDecimal(oRecordSetD.Fields.Item("MontoDescuento").Value).ToString(sDecimal).Replace(",", "") > 0 Then
                                            sXML &= "            <dte:Descuento>" & Convert.ToDecimal((nOp3 - nOp2) * 1).ToString(sDecimal).Replace(",", "") & "</dte:Descuento>"
                                        Else
                                            sXML &= "            <dte:Descuento>0</dte:Descuento>"
                                        End If
                                    End If

                                    If Not oRecordSetSERIESO.Fields.Item("U_Type").Value = "NABN" Then
                                        sXML &= "            <dte:Impuestos>"
                                        sXML &= "              <dte:Impuesto>"
                                        sXML &= "                <dte:NombreCorto>IVA</dte:NombreCorto>"

                                        If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Or oRecordSetSERIESO.Fields.Item("U_Type").Value = "CAXP" Or oRecordSetD.Fields.Item("TaxCode").Value = "EXE" Then
                                            sXML &= "                <dte:CodigoUnidadGravable>2</dte:CodigoUnidadGravable>"
                                        Else
                                            sXML &= "                <dte:CodigoUnidadGravable>1</dte:CodigoUnidadGravable>"
                                        End If

                                        pPrecioUni = oRecordSetD.Fields.Item("PriceAfVat").Value


                                        If oRecordSetH.Fields.Item("DocType").Value = "I" Then
                                            dQty = oRecordSetD.Fields.Item("Quantity").Value
                                        ElseIf oRecordSetH.Fields.Item("DocType").Value = "S" Then
                                            dQty = 1
                                        End If


                                        If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                            pPrecio = Math.Round((pPrecioUni * dQty), 4)
                                        Else
                                            pPrecio = Math.Round((pPrecioUni * dQty), 4)
                                        End If

                                        If oRecordSetD.Fields.Item("TaxCode").Value = "EXE" Then
                                            pMontoG = Math.Round(pPrecio, 4)
                                            pIVA = 0.00
                                        Else
                                            pMontoG = Math.Round((pPrecio / 1.12), 4)
                                            pIVA = Math.Round((pPrecio - pMontoG), 4)
                                        End If

                                        If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Or oRecordSetSERIESO.Fields.Item("U_Type").Value = "CAXP" Or oRecordSetD.Fields.Item("TaxCode").Value = "EXE" Then

                                            If Convert.ToDecimal(oRecordSetD.Fields.Item("MontoDescuento").Value).ToString(sDecimal).Replace(",", "") > 0 Then

                                                If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                                    sXML &= "                <dte:MontoGravable>" & Convert.ToDecimal(oRecordSetD.Fields.Item("GTotal").Value).ToString(sDecimal).Replace(",", "") & "</dte:MontoGravable>"
                                                Else
                                                    sXML &= "                <dte:MontoGravable>" & Convert.ToDecimal(oRecordSetD.Fields.Item("GTotalFC").Value).ToString(sDecimal).Replace(",", "") & "</dte:MontoGravable>"
                                                End If

                                            Else

                                                If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                                    sXML &= "                <dte:MontoGravable>" & Convert.ToDecimal(oRecordSetD.Fields.Item("GTotal").Value).ToString(sDecimal).Replace(",", "") & "</dte:MontoGravable>"
                                                Else
                                                    sXML &= "                <dte:MontoGravable>" & Convert.ToDecimal(oRecordSetD.Fields.Item("GTotalFC").Value).ToString(sDecimal).Replace(",", "") & "</dte:MontoGravable>"
                                                End If

                                            End If
                                        Else
                                            sXML &= "                <dte:MontoGravable>" & Convert.ToDecimal(num10).ToString(sDecimal).Replace(",", "") & "</dte:MontoGravable>"
                                        End If



                                        If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Or oRecordSetSERIESO.Fields.Item("U_Type").Value = "CAXP" Or oRecordSetD.Fields.Item("TaxCode").Value = "EXE" Then
                                            sXML &= "                <dte:MontoImpuesto>0</dte:MontoImpuesto>"
                                        Else

                                            sXML &= "                <dte:MontoImpuesto>" & num11 & "</dte:MontoImpuesto>"
                                        End If
                                        sXML &= "              </dte:Impuesto>"
                                        sXML &= "            </dte:Impuestos>"
                                    End If

                                    If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                        sXML &= "            <dte:Total>" & Convert.ToDecimal(oRecordSetD.Fields.Item("GTotal").Value).ToString(sDecimal).Replace(",", "") & "</dte:Total>"
                                    Else
                                        sXML &= "            <dte:Total>" & Convert.ToDecimal(oRecordSetD.Fields.Item("GTotalFC").Value).ToString(sDecimal).Replace(",", "") & "</dte:Total>"
                                    End If

                                    '************ PERSONALIZADOS DETALLE ************

                                    If oRecordSetFEL.Fields.Item("U_PerDetalle").Value.ToString() = "Y" Then

                                        If Query_Manager_P("dte:personalizado1", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado1></dte:personalizado1>"
                                        Else
                                            sXML &= "            <dte:personalizado1>" & Query_Manager_P("dte:personalizado1", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado1>"
                                        End If

                                        If Query_Manager_P("dte:personalizado2", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado2></dte:personalizado2>"
                                        Else
                                            sXML &= "            <dte:personalizado2>" & Query_Manager_P("dte:personalizado2", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado2>"
                                        End If

                                        If Query_Manager_P("dte:personalizado3", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado3></dte:personalizado3>"
                                        Else
                                            sXML &= "            <dte:personalizado3>" & Query_Manager_P("dte:personalizado3", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado3>"
                                        End If

                                        If Query_Manager_P("dte:personalizado4", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado4></dte:personalizado4>"
                                        Else
                                            sXML &= "            <dte:personalizado4>" & Query_Manager_P("dte:personalizado4", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado4>"
                                        End If

                                        If Query_Manager_P("dte:personalizado5", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado5></dte:personalizado5>"
                                        Else
                                            sXML &= "            <dte:personalizado5>" & Query_Manager_P("dte:personalizado5", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado5>"
                                        End If

                                        If Query_Manager_P("dte:personalizado6", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado6></dte:personalizado6>"
                                        Else
                                            sXML &= "            <dte:personalizado6>" & Query_Manager_P("dte:personalizado6", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado6>"
                                        End If

                                        If Query_Manager_P("dte:personalizado7", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado7></dte:personalizado7>"
                                        Else
                                            sXML &= "            <dte:personalizado7>" & Query_Manager_P("dte:personalizado7", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado7>"
                                        End If

                                        If Query_Manager_P("dte:personalizado8", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado8></dte:personalizado8>"
                                        Else
                                            sXML &= "            <dte:personalizado8>" & Query_Manager_P("dte:personalizado8", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado8>"
                                        End If

                                        If Query_Manager_P("dte:personalizado9", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado9></dte:personalizado9>"
                                        Else
                                            sXML &= "            <dte:personalizado9>" & Query_Manager_P("dte:personalizado5", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado9>"
                                        End If

                                        If Query_Manager_P("dte:personalizado10", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado10></dte:personalizado10>"
                                        Else
                                            sXML &= "            <dte:personalizado10>" & Query_Manager_P("dte:personalizado5", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado10>"
                                        End If

                                        If Query_Manager_P("dte:personalizado11", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado11></dte:personalizado11>"
                                        Else
                                            sXML &= "            <dte:personalizado11>" & Query_Manager_P("dte:personalizado11", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado11>"
                                        End If

                                        If Query_Manager_P("dte:personalizado12", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado12></dte:personalizado12>"
                                        Else
                                            sXML &= "            <dte:personalizado12>" & Query_Manager_P("dte:personalizado12", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado12>"
                                        End If

                                        If Query_Manager_P("dte:personalizado13", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado13></dte:personalizado13>"
                                        Else
                                            sXML &= "            <dte:personalizado13>" & Query_Manager_P("dte:personalizado13", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado13>"
                                        End If

                                        If Query_Manager_P("dte:personalizado14", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado14></dte:personalizado14>"
                                        Else
                                            sXML &= "            <dte:personalizado14>" & Query_Manager_P("dte:personalizado14", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado14>"
                                        End If

                                        If Query_Manager_P("dte:personalizado15", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                                            sXML &= "            <dte:personalizado15></dte:personalizado15>"
                                        Else
                                            sXML &= "            <dte:personalizado15>" & Query_Manager_P("dte:personalizado15", oRecordSetH.Fields.Item("DocEntry").Value, iCont.ToString(), oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:personalizado15>"
                                        End If

                                    End If

                                    'FIN PERSONALIZADOS DETALLE

                                    sXML &= "          </dte:Item>"



                                    If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Or oRecordSetSERIESO.Fields.Item("U_Type").Value = "CAXP" Or oRecordSetD.Fields.Item("TaxCode").Value = "EXE" Then
                                        SumaIva += 0
                                    Else
                                        SumaIva += pIVA
                                    End If

                                    SumaPrecioUnitario += Convert.ToDecimal(nOp3)

                                    If oRecordSetH.Fields.Item("DocType").Value = "I" Then
                                        SumaPrecio += Convert.ToDecimal(nOp3 * oRecordSetD.Fields.Item("Quantity").Value)

                                    ElseIf oRecordSetH.Fields.Item("DocType").Value = "S" Then
                                        SumaPrecio += Convert.ToDecimal(nOp3 * 1)

                                    End If

                                    If Convert.ToDecimal(oRecordSetD.Fields.Item("MontoDescuento").Value).ToString(sDecimal).Replace(",", "") > 0 Then
                                        SumaDescuento += Convert.ToDecimal((nOp3 - nOp2) * Convert.ToDecimal(oRecordSetD.Fields.Item("Quantity").Value).ToString(sDecimal).Replace(",", ""))
                                    Else
                                        SumaDescuento += 0
                                    End If

                                    If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Or oRecordSetD.Fields.Item("TaxCode").Value = "EXE" Then

                                        If Convert.ToDecimal(oRecordSetD.Fields.Item("MontoDescuento").Value).ToString(sDecimal).Replace(",", "") > 0 Then

                                            If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                                SumaMontoGravable += pMontoG
                                            Else
                                                SumaMontoGravable += pMontoG
                                            End If
                                        Else
                                            SumaMontoGravable += SumaPrecio

                                        End If
                                    Else

                                        If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                            SumaMontoGravable += pMontoG
                                        Else
                                            SumaMontoGravable += pMontoG
                                        End If

                                    End If

                                    If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                        SumaTotal += Convert.ToDecimal(oRecordSetD.Fields.Item("GTotal").Value).ToString(sDecimal)
                                    Else
                                        SumaTotal += Convert.ToDecimal(oRecordSetD.Fields.Item("GTotalFC").Value).ToString(sDecimal)
                                    End If

                                    iLineaContador = iLineaContador + 1

                                    oRecordSetD.MoveNext()
                                Else

                                    If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Or oRecordSetD.Fields.Item("TaxCode").Value = "EXE" Then
                                        SumaIva += 0
                                    Else
                                        SumaIva += pIVA
                                    End If

                                    SumaPrecioUnitario += Convert.ToDecimal(nOp3 * oRecordSetD.Fields.Item("Quantity").Value)

                                    If oRecordSetH.Fields.Item("DocType").Value = "I" Then
                                        SumaPrecio += Convert.ToDecimal(nOp3 * oRecordSetD.Fields.Item("Quantity").Value)

                                    ElseIf oRecordSetH.Fields.Item("DocType").Value = "S" Then
                                        SumaPrecio += Convert.ToDecimal(nOp3 * 1)

                                    End If

                                    SumaDescuento += Convert.ToDecimal((nOp3 - nOp2) * Convert.ToDecimal(oRecordSetD.Fields.Item("Quantity").Value).ToString(sDecimal).Replace(",", ""))

                                    If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Or oRecordSetD.Fields.Item("TaxCode").Value = "EXE" Then

                                        If Convert.ToDecimal(oRecordSetD.Fields.Item("MontoDescuento").Value).ToString(sDecimal).Replace(",", "") > 0 Then

                                            If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                                SumaMontoGravable += pMontoG
                                            Else
                                                SumaMontoGravable += pMontoG
                                            End If
                                        Else
                                            SumaMontoGravable += SumaPrecio
                                        End If
                                    Else
                                        If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                            SumaMontoGravable += pMontoG
                                        Else
                                            SumaMontoGravable += pMontoG
                                        End If
                                    End If

                                    If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                        SumaTotal += Convert.ToDecimal(oRecordSetD.Fields.Item("GTotal").Value).ToString(sDecimal)
                                    Else
                                        SumaTotal += Convert.ToDecimal(oRecordSetD.Fields.Item("GTotalFC").Value).ToString(sDecimal)
                                    End If

                                    oRecordSetD.MoveNext()

                                End If
                            Else
                                oRecordSetD.MoveNext()
                            End If
                        Next

                    End If

                    If oBanderaH = True Then

                        sXML &= "          <dte:Item NumeroLinea=" & Chr(34) & iLineaContador & Chr(34) & " BienOServicio=""B"">"
                        sXML &= "            <dte:Cantidad>1</dte:Cantidad>"
                        sXML &= "            <dte:UnidadMedida>UND</dte:UnidadMedida>"
                        sXML &= "            <dte:Descripcion>" & oRecordSetH.Fields.Item("Header").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:Descripcion>"
                        sXML &= "            <dte:PrecioUnitario>" & Convert.ToDecimal(SumaPrecioUnitario).ToString(sDecimal).Replace(",", "") & "</dte:PrecioUnitario>"
                        sXML &= "            <dte:Precio>" & Convert.ToDecimal(SumaPrecio).ToString(sDecimal).Replace(",", "") & "</dte:Precio>"
                        sXML &= "            <dte:Descuento>" & Convert.ToDecimal(SumaDescuento).ToString(sDecimal).Replace(",", "") & "</dte:Descuento>"

                        If Not oRecordSetSERIESO.Fields.Item("U_Type").Value = "NABN" Then

                            sXML &= "            <dte:Impuestos>"
                            sXML &= "              <dte:Impuesto>"
                            sXML &= "                <dte:NombreCorto>IVA</dte:NombreCorto>"
                            sXML &= "                <dte:CodigoUnidadGravable>1</dte:CodigoUnidadGravable>"

                            'If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Or oRecordSetD.Fields.Item("TaxCode").Value = "EXE" Then
                            If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Then
                                sXML &= "                <dte:MontoGravable>" & Convert.ToDecimal(SumaPrecio).ToString(sDecimal).Replace(",", "") & "</dte:MontoGravable>"
                            Else
                                sXML &= "                <dte:MontoGravable>" & Convert.ToDecimal(SumaMontoGravable).ToString(sDecimal).Replace(",", "") & "</dte:MontoGravable>"
                            End If

                            If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Then
                                'If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Or oRecordSetD.Fields.Item("TaxCode").Value = "EXE" Then
                                sXML &= "                <dte:MontoImpuesto>0</dte:MontoImpuesto>"
                            Else
                                sXML &= "                <dte:MontoImpuesto>" & Convert.ToDecimal(SumaIva).ToString(sDecimal).Replace(",", "") & "</dte:MontoImpuesto>"
                            End If

                            sXML &= "              </dte:Impuesto>"
                            sXML &= "            </dte:Impuestos>"

                        End If

                        sXML &= "            <dte:Total>" & Convert.ToDecimal(SumaTotal).ToString(sDecimal).Replace(",", "") & "</dte:Total>"
                        sXML &= "          </dte:Item>"

                    End If

                    'GASTOS ADICIONALES

                    oRecordSetGAdicionales = Nothing
                    oRecordSetGAdicionales = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

                    If TipoDoc = "13" Then

                        sQuery = " SELECT T2.""ExpnsName"",T1.""LineTotal"", T1.""TaxCode"" FROM OINV T0 INNER JOIN INV3 T1 ON T0.""DocEntry"" = T1.""DocEntry"""
                        sQuery &= " INNER JOIN OEXD T2 ON T1.""ExpnsCode"" = T2.""ExpnsCode"" "
                        sQuery &= " WHERE T0.""DocEntry"" = " & sDocEntry

                    ElseIf (TipoDoc = "14") Then

                        sQuery = " SELECT T2.""ExpnsName"",T1.""LineTotal"", T1.""TaxCode"" FROM ORIN T0 INNER JOIN RIN3 T1 ON T0.""DocEntry"" = T1.""DocEntry"" "
                        sQuery &= " INNER JOIN OEXD T2 ON T1.""ExpnsCode"" = T2.""ExpnsCode"" "
                        sQuery &= " WHERE T0.""DocEntry"" = " & sDocEntry

                    ElseIf (TipoDoc = "DN") Then

                        sQuery = " SELECT T2.""ExpnsName"",T1.""LineTotal"", T1.""TaxCode"" FROM OINV T0 INNER JOIN INV3 T1 ON T0.""DocEntry"" = T1.""DocEntry"""
                        sQuery &= " INNER JOIN OEXD T2 ON T1.""ExpnsCode"" = T2.""ExpnsCode"" "
                        sQuery &= " WHERE T0.""DocEntry"" = " & sDocEntry

                    End If

                    'PREGUNTAR SOBRE VALIDACION O SI SIEMPRE DEBE SER UTILIZADO 

                    oRecordSetGAdicionales.DoQuery(sQuery)

                    For iContGastos As Integer = 0 To oRecordSetGAdicionales.RecordCount - 1
                        Dim mGravable As Double = 0.00
                        Dim mIva As Double = 0.00
                        Dim mTotalGasto As Double = 0.00
                        Dim mImpuesto As String = ""
                        Dim mCodGravable As Integer = 0

                        If oRecordSetGAdicionales.Fields.Item("TaxCode").Value.ToString = "ENV" Then
                            mCodGravable = 1
                            mGravable = (oRecordSetGAdicionales.Fields.Item("LineTotal").Value / 1.12)
                            mIva = Math.Round((mGravable * 0.12), 2)

                        Else
                            mCodGravable = 2
                            mGravable = oRecordSetGAdicionales.Fields.Item("LineTotal").Value
                            mIva = 0.00

                        End If

                        sXML &= " <dte:Item NumeroLinea=" & Chr(34) & iLineaContador & Chr(34) & " BienOServicio=""B"">"
                        sXML &= "   <dte:Cantidad>1</dte:Cantidad>"
                        sXML &= "   <dte:UnidadMedida>UND</dte:UnidadMedida>"
                        sXML &= "   <dte:Descripcion>" & "          | " & oRecordSetGAdicionales.Fields.Item("ExpnsName").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</dte:Descripcion>"
                        sXML &= "   <dte:PrecioUnitario>" & Math.Round(oRecordSetGAdicionales.Fields.Item("LineTotal").Value, 2) & "</dte:PrecioUnitario>"
                        sXML &= "   <dte:Precio>" & Math.Round(oRecordSetGAdicionales.Fields.Item("LineTotal").Value, 2) & "</dte:Precio>"
                        sXML &= "   <dte:Descuento>0.00</dte:Descuento>"
                        sXML &= "       <dte:Impuestos>"
                        sXML &= "           <dte:Impuesto>"
                        sXML &= "               <dte:NombreCorto>" & oRecordSetGAdicionales.Fields.Item("TaxCode").Value.ToString & "</dte:NombreCorto> "
                        sXML &= "               <dte:CodigoUnidadGravable>" & Math.Round(mCodGravable, 2) & "</dte:CodigoUnidadGravable> "
                        sXML &= "               <dte:MontoGravable>" & Math.Round(mGravable, 2) & "</dte:MontoGravable>"
                        sXML &= "               <dte:MontoImpuesto>" & Math.Round(mIva, 2) & "</dte:MontoImpuesto>"
                        sXML &= "           </dte:Impuesto>"
                        sXML &= "       </dte:Impuestos>"
                        sXML &= "  <dte:Total>" & oRecordSetGAdicionales.Fields.Item("LineTotal").Value & "</dte:Total>"
                        sXML &= "</dte:Item>"

                        oRecordSetGAdicionales.MoveNext()
                        SumaIva += mIva

                    Next

                    'FIN GASTOS ADICIONALES
                    sXML &= "        </dte:Items>"




                    sXML &= "        <dte:Totales>"
                    If AgrupaDetalle = "SI" Then
                        sXML &= "          <dte:TotalImpuestos>"
                        sXML &= "            <dte:TotalImpuesto NombreCorto=""IVA"" TotalMontoImpuesto=" & Chr(34) & ImpuestoAgrupado & Chr(34) & " />"
                        sXML &= "          </dte:TotalImpuestos>"
                    Else
                        If Not oRecordSetSERIESO.Fields.Item("U_Type").Value = "NABN" Then
                            sXML &= "          <dte:TotalImpuestos>"
                            If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Or oRecordSetSERIESO.Fields.Item("U_Type").Value = "CAXP" Then
                                sXML &= "            <dte:TotalImpuesto NombreCorto=""IVA"" TotalMontoImpuesto=""0"" />"
                            Else
                                sXML &= "            <dte:TotalImpuesto NombreCorto=""IVA"" TotalMontoImpuesto=" & Chr(34) & Convert.ToDecimal(SumaIva).ToString().Replace(",", "") & Chr(34) & " />"
                            End If
                            sXML &= "          </dte:TotalImpuestos>"
                        End If
                    End If

                    If AgrupaDetalle = "SI" Then
                        sXML &= "          <dte:GranTotal>" & TotalAgrupado & "</dte:GranTotal>"
                    Else
                        If oRecordSetSERIESO.Fields.Item("U_Type").Value = "FESP" Then
                            sXML &= "          <dte:GranTotal>" & Convert.ToDecimal(SumaPrecio).ToString(sDecimal).Replace(",", "") & "</dte:GranTotal>"
                        Else
                            If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                sXML &= "          <dte:GranTotal>" & Convert.ToDecimal(oRecordSetH.Fields.Item("DocTotal").Value).ToString(sDecimal).Replace(",", "") & "</dte:GranTotal>"
                            ElseIf oRecordSetH.Fields.Item("DocCur").Value = "USD" Then
                                sXML &= "          <dte:GranTotal>" & Convert.ToDecimal(oRecordSetH.Fields.Item("DocTotalFC").Value).ToString(sDecimal).Replace(",", "") & "</dte:GranTotal>"
                            End If
                        End If
                    End If


                    sXML &= "        </dte:Totales>"

                    '**********COBRO POR CUENTA AJENA******************

                    If oRecordSetFEL.Fields.Item("U_CobroAje").Value = "Y" Then

                        'Declaro recordset sobre cuenta ajena G = Datos generales, D = Datos detalle

                        Dim oRecordSetCuentaG As SAPbobsCOM.Recordset
                        Dim oRecordSetCuentaD As SAPbobsCOM.Recordset

                        oRecordSetCuentaG = Nothing
                        oRecordSetCuentaG = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select T0.""U_NITCA"", T0.""U_NumeroCA,"", T0.""U_FechaCA"", T0.* "
                        sQuery &= "FROM OINV T0 Where T0.""DocEntry"" = " & oRecordSetH.Fields.Item("DocEntry").Value
                        oRecordSetCuentaG.DoQuery(sQuery)
                        oRecordSetCuentaG.MoveFirst()



                        If ProcesaCuentaAjena = "Y" Then


                            oRecordSetCuentaD = Nothing
                            oRecordSetCuentaD = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                            sQuery = "Select T1.""U_DescripcionCA"", T1.""U_ImponibleCA"", T1.""U_MontoDAI"", "
                            sQuery &= "T1.""U_IVACA"", T1.""U_OtrosCA"", T1.""U_TotalCA"" "
                            sQuery &= " FROM INV1 T1 INNER JOIN OINV T0 ON T1.""DocEntry"" = T0.""DocEntry"" Where T0.""DocEntry"" = " & oRecordSetH.Fields.Item("DocEntry").Value
                            sQuery &= "Where T1.""VisOrder"" = " & LineaCuentaAjena
                            oRecordSetCuentaD.DoQuery(sQuery)
                            oRecordSetCuentaD.MoveFirst()

                            If oRecordSetCuentaD.RecordCount > 0 Then


                                sXML &= "				<dte:Complementos> "
                                sXML &= "					<dte:Complemento NombreComplemento=""GT_Complemento_CobroXCuentaAjena"" URIComplemento=""http://www.sat.gob.gt/face2/CobroXCuentaAjena/0.1.0""> "
                                sXML &= "						<cca:CobroXCuentaAjena Version=""1""> "
                                sXML &= "						    <cca:ItemCuentaAjena> "
                                sXML &= "							    <cca:NITtercero>" & oRecordSetCuentaG.Fields.Item("U_NITCA").Value.ToString & "</cca:NITtercero> "
                                sXML &= "							    <cca:NumeroDocumento>" & oRecordSetCuentaG.Fields.Item("U_NumeroCA").Value.ToString & "</cca:NumeroDocumento> "
                                sXML &= "							    <cca:FechaDocumento>" & oRecordSetCuentaG.Fields.Item("U_FechaCA").Value.ToString & "</cca:FechaDocumento> "
                                sXML &= "							    <cca:Descripcion>" & oRecordSetCuentaD.Fields.Item("U_DescripcionCA").Value.ToString & "</cca:Descripcion> "
                                sXML &= "							    <cca:BaseImponible>" & oRecordSetCuentaD.Fields.Item("U_ImponibleCA").Value.ToString & "</cca:BaseImponible> "
                                sXML &= "							    <cca:MontoCobroDAI>" & oRecordSetCuentaD.Fields.Item("U_MontoDAI").Value.ToString & "</cca:MontoCobroDAI> "
                                sXML &= "							    <cca:MontoCobroIVA>" & oRecordSetCuentaD.Fields.Item("U_IVACA").Value.ToString & "</cca:MontoCobroIVA> "
                                sXML &= "							    <cca:MontoCobroOtros>" & oRecordSetCuentaD.Fields.Item("U_OtrosCA").Value.ToString & "</cca:MontoCobroOtros> "
                                sXML &= "							    <cca:MontoCobroTotal>" & oRecordSetCuentaD.Fields.Item("U_TotalCA").Value.ToString & "</cca:MontoCobroTotal> "
                                sXML &= "						    </cca:ItemCuentaAjena> "
                                sXML &= "						</cca:CobroXCuentaAjena> "
                                sXML &= "					</dte:Complemento> "
                                sXML &= "				</dte:Complementos> "

                            End If

                        End If

                    End If


                    If oRecordSetSERIESO.Fields.Item("U_Type").Value = "FESP" Then

                        sXML &= "				<dte:Complementos> "
                        sXML &= "					<dte:Complemento NombreComplemento=""GT_Complemento_Fac_Especial"" URIComplemento=""http://www.sat.gob.gt/face2/ComplementoFacturaEspecial/0.1.0""> "
                        sXML &= "						<cfe:RetencionesFacturaEspecial Version=""1""> "

                        Dim oRecordSetISR As SAPbobsCOM.Recordset
                        Dim oRecordSetIVA As SAPbobsCOM.Recordset

                        Dim nResta As Decimal = 0.0

                        oRecordSetISR = Nothing
                        oRecordSetISR = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select Sum(T0.""WTAmnt"") from PCH5 T0 Where T0.""WTCode"" like '%ISR%' and T0.""AbsEntry"" = " & oRecordSetH.Fields.Item("DocEntry").Value
                        oRecordSetISR.DoQuery(sQuery)
                        oRecordSetISR.MoveFirst()

                        sXML &= "							<cfe:RetencionISR>" & Convert.ToDecimal(oRecordSetISR.Fields.Item(0).Value).ToString(sDecimal).Replace(",", "") & "</cfe:RetencionISR> "

                        oRecordSetIVA = Nothing
                        oRecordSetIVA = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select Sum(T0.""WTAmnt"") from PCH5 T0 Where T0.""WTCode"" not like '%ISR%' and T0.""AbsEntry"" = " & oRecordSetH.Fields.Item("DocEntry").Value
                        oRecordSetIVA.DoQuery(sQuery)
                        oRecordSetIVA.MoveFirst()

                        sXML &= "							<cfe:RetencionIVA>" & Convert.ToDecimal(oRecordSetIVA.Fields.Item(0).Value).ToString(sDecimal).Replace(",", "") & "</cfe:RetencionIVA> "

                        nResta = Convert.ToDecimal(SumaPrecio).ToString(sDecimal).Replace(",", "") - Convert.ToDecimal(oRecordSetISR.Fields.Item(0).Value).ToString(sDecimal).Replace(",", "")
                        nResta = nResta - Convert.ToDecimal(oRecordSetIVA.Fields.Item(0).Value).ToString(sDecimal).Replace(",", "")

                        sXML &= "							<cfe:TotalMenosRetenciones>" & Convert.ToDecimal(nResta).ToString(sDecimal).Replace(",", "") & "</cfe:TotalMenosRetenciones> "
                        sXML &= "						</cfe:RetencionesFacturaEspecial> "
                        sXML &= "					</dte:Complemento> "
                        sXML &= "				</dte:Complementos> "

                    End If

                    If oRecordSetSERIESO.Fields.Item("U_Type").Value = "EXPO" Then

                        sXML &= "    <dte:Complementos>"
                        sXML &= "    <dte:Complemento NombreComplemento=""GT_Complemento_Exportaciones"" URIComplemento=""http://www.sat.gob.gt/face2/ComplementoExportaciones/0.1.0""> "
                        sXML &= "    <cex:Exportacion Version=""1""> "

                        If Query_Manager_H("cex:NombreConsignatarioODestinatario", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:NombreConsignatarioODestinatario>" & oRecordSetH.Fields.Item("CardName").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:NombreConsignatarioODestinatario>"
                        Else
                            sXML &= "    <cex:NombreConsignatarioODestinatario>" & Query_Manager_H("cex:DireccionConsignatarioODestinatario", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:NombreConsignatarioODestinatario>"
                        End If

                        If Query_Manager_H("cex:DireccionConsignatarioODestinatario", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:DireccionConsignatarioODestinatario>" & quitarSaltosLinea(oRecordSetH.Fields.Item("Address").Value, " ").ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:DireccionConsignatarioODestinatario>"
                        Else
                            sXML &= "    <cex:DireccionConsignatarioODestinatario>" & Query_Manager_H("cex:DireccionConsignatarioODestinatario", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:DireccionConsignatarioODestinatario>"
                        End If

                        If Query_Manager_H("cex:CodigoConsignatarioODestinatario", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:CodigoConsignatarioODestinatario>" & oRecordSetH.Fields.Item("CardCode").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:CodigoConsignatarioODestinatario>"
                        Else
                            sXML &= "    <cex:CodigoConsignatarioODestinatario>" & Query_Manager_H("cex:CodigoConsignatarioODestinatario", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:CodigoConsignatarioODestinatario>"
                        End If

                        If Query_Manager_H("cex:NombreComprador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:NombreComprador>" & oRecordSetH.Fields.Item("CardName").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:NombreComprador>"
                        Else
                            sXML &= "    <cex:NombreComprador>" & Query_Manager_H("cex:NombreComprador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:NombreComprador>"
                        End If

                        If Query_Manager_H("cex:DireccionComprador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:DireccionComprador>" & oRecordSetH.Fields.Item("Address").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:DireccionComprador>"
                        Else
                            sXML &= "    <cex:DireccionComprador>" & Query_Manager_H("cex:DireccionComprador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:DireccionComprador>"
                        End If

                        If Query_Manager_H("cex:CodigoComprador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:CodigoComprador>" & oRecordSetH.Fields.Item("CardCode").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:CodigoComprador>"
                        Else
                            sXML &= "    <cex:CodigoComprador>" & Query_Manager_H("cex:CodigoComprador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:CodigoComprador>"
                        End If

                        If Query_Manager_H("cex:OtraReferencia", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:OtraReferencia>" & oRecordSetH.Fields.Item("Comments").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:OtraReferencia>"
                        Else
                            sXML &= "    <cex:OtraReferencia>" & Query_Manager_H("cex:OtraReferencia", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:OtraReferencia>"
                        End If

                        If String.IsNullOrEmpty(oRecordSetH.Fields.Item("U_Incoterms").Value) Then
                            B1Connections.theAppl.MessageBox("Seleccione Un Incoterms")
                            oArchivo.Close()
                            oArchivo.Flush()
                            Exit Try
                        Else
                            sXML &= "    <cex:INCOTERM>" & oRecordSetH.Fields.Item("U_Incoterms").Value.ToString() & "</cex:INCOTERM>"
                        End If

                        If Query_Manager_H("cex:NombreExportador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:NombreExportador></cex:NombreExportador>"
                        Else
                            sXML &= "    <cex:NombreExportador>" & Query_Manager_H("cex:NombreExportador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:NombreExportador>"
                        End If

                        If Query_Manager_H("cex:CodigoExportador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:CodigoExportador></cex:CodigoExportador>"
                        Else
                            sXML &= "    <cex:CodigoExportador>" & Query_Manager_H("cex:CodigoExportador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:CodigoExportador>"
                        End If

                        sXML &= "    </cex:Exportacion>"
                        sXML &= "   </dte:Complemento>"
                        sXML &= "  </dte:Complementos>"

                    End If

                    'FACTURA CAMBIARIA DE EXPORTACION LLEVA DOS COMPLEMENTOS
                    If oRecordSetSERIESO.Fields.Item("U_Type").Value = "CAXP" Then

                        sXML &= "    <dte:Complementos>"
                        sXML &= "    <dte:Complemento NombreComplemento=""GT_Complemento_Exportaciones"" URIComplemento=""http://www.sat.gob.gt/face2/ComplementoExportaciones/0.1.0""> "
                        sXML &= "    <cex:Exportacion Version=""1""> "


                        If Query_Manager_H("cex:NombreConsignatarioODestinatario", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:NombreConsignatarioODestinatario>" & oRecordSetH.Fields.Item("CardName").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:NombreConsignatarioODestinatario>"
                        Else
                            sXML &= "    <cex:NombreConsignatarioODestinatario>" & Query_Manager_H("cex:NombreConsignatarioODestinatario", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:NombreConsignatarioODestinatario>"
                        End If


                        If Query_Manager_H("cex:DireccionConsignatarioODestinatario", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then

                            sXML &= "    <cex:DireccionConsignatarioODestinatario>CIUDAD</cex:DireccionConsignatarioODestinatario>"
                        Else

                            sXML &= "    <cex:DireccionConsignatarioODestinatario>" & Query_Manager_H("cex:DireccionConsignatarioODestinatario", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:DireccionConsignatarioODestinatario>"
                        End If

                        If Query_Manager_H("cex:CodigoConsignatarioODestinatario", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:CodigoConsignatarioODestinatario>" & oRecordSetH.Fields.Item("CardCode").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:CodigoConsignatarioODestinatario>"
                        Else
                            sXML &= "    <cex:CodigoConsignatarioODestinatario>" & Query_Manager_H("cex:CodigoConsignatarioODestinatario", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:CodigoConsignatarioODestinatario>"
                        End If

                        If Query_Manager_H("cex:NombreComprador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:NombreComprador>" & oRecordSetH.Fields.Item("CardName").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:NombreComprador>"
                        Else
                            sXML &= "    <cex:NombreComprador>" & Query_Manager_H("cex:NombreComprador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:NombreComprador>"
                        End If

                        If Query_Manager_H("cex:DireccionComprador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:DireccionComprador>Ciudad</cex:DireccionComprador>"
                        Else
                            sXML &= "    <cex:DireccionComprador>" & Query_Manager_H("cex:DireccionComprador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:DireccionComprador>"
                        End If

                        If Query_Manager_H("cex:CodigoComprador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:CodigoComprador>" & oRecordSetH.Fields.Item("CardCode").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:CodigoComprador>"
                        Else
                            sXML &= "    <cex:CodigoComprador>" & Query_Manager_H("cex:CodigoComprador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:CodigoComprador>"
                        End If

                        If Query_Manager_H("cex:OtraReferencia", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:OtraReferencia>" & oRecordSetH.Fields.Item("Comments").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:OtraReferencia>"
                        Else
                            sXML &= "    <cex:OtraReferencia>" & Query_Manager_H("cex:OtraReferencia", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:OtraReferencia>"
                        End If

                        If String.IsNullOrEmpty(oRecordSetH.Fields.Item("U_Incoterms").Value) Then
                            B1Connections.theAppl.MessageBox("Seleccione Un Incoterms")
                            oArchivo.Close()
                            oArchivo.Flush()
                            Exit Try
                        Else
                            sXML &= "    <cex:INCOTERM>" & oRecordSetH.Fields.Item("U_Incoterms").Value.ToString() & "</cex:INCOTERM>"
                        End If

                        If Query_Manager_H("cex:NombreExportador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:NombreExportador></cex:NombreExportador>"
                        Else
                            sXML &= "    <cex:NombreExportador>" & Query_Manager_H("cex:NombreExportador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:NombreExportador>"
                        End If

                        If Query_Manager_H("cex:CodigoExportador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "    <cex:CodigoExportador></cex:CodigoExportador>"
                        Else
                            sXML &= "    <cex:CodigoExportador>" & Query_Manager_H("cex:CodigoExportador", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cex:CodigoExportador>"
                        End If

                        sXML &= "    </cex:Exportacion>"
                        sXML &= "   </dte:Complemento>"

                        sXML &= "       <dte:Complemento NombreComplemento=""GT_Complemento_Cambiaria"" URIComplemento=""http://www.sat.gob.gt/dte/fel/CompCambiaria/0.2.0"">"
                        sXML &= "           <cfc:AbonosFacturaCambiaria Version=""1"" > "
                        sXML &= "               <cfc:Abono>"

                        If Query_Manager_H("cfc:NumeroAbono", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "               <cfc:NumeroAbono>1</cfc:NumeroAbono>"
                        Else
                            sXML &= "                   <cfc:NumeroAbono>" & Query_Manager_H("cfc:NumeroAbono", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cfc:NumeroAbono>"
                        End If

                        If Query_Manager_H("cfc:FechaVencimiento", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "               <cfc:FechaVencimiento>" & CDate(oRecordSetH.Fields.Item("DocDueDate").Value).ToString("yyyy-MM-dd") & "</cfc:FechaVencimiento>"
                        Else
                            sXML &= "                   <cfc:FechaVencimiento>" & Query_Manager_H("cfc:FechaVencimiento", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cfc:FechaVencimiento>"
                        End If


                        If Query_Manager_H("cfc:MontoAbono", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then

                            If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                sXML &= "               <cfc:MontoAbono>" & Convert.ToDecimal(oRecordSetH.Fields.Item("DocTotal").Value).ToString(sDecimal).Replace(",", "") & "</cfc:MontoAbono>"
                            ElseIf oRecordSetH.Fields.Item("DocCur").Value = "USD" Then
                                sXML &= "               <cfc:MontoAbono>" & Convert.ToDecimal(oRecordSetH.Fields.Item("DocTotalFC").Value).ToString(sDecimal).Replace(",", "") & "</cfc:MontoAbono>"
                            End If

                        Else
                            sXML &= "                   <cfc:MontoAbono>" & Convert.ToDecimal(Query_Manager_H("cfc:MontoAbono", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;")).ToString(sDecimal).Replace(",", "") & "</cfc:MontoAbono>"
                        End If

                        sXML &= "               </cfc:Abono>"
                        sXML &= "           </cfc:AbonosFacturaCambiaria>"
                        sXML &= "     </dte:Complemento>"
                        sXML &= "  </dte:Complementos>"

                    End If

                    If oRecordSetSERIESO.Fields.Item("U_Type").Value = "FCAM" Then

                        sXML &= "   <dte:Complementos>"
                        sXML &= "       <dte:Complemento NombreComplemento=""GT_Complemento_Cambiaria.xsd"" URIComplemento=""http://www.sat.gob.gt/dte/fel/CompCambiaria/0.2.0"">"
                        sXML &= "           <cfc:AbonosFacturaCambiaria Version=""1""> "
                        sXML &= "               <cfc:Abono>"

                        If Query_Manager_H("cfc:NumeroAbono", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "               <cfc:NumeroAbono>1</cfc:NumeroAbono>"
                        Else
                            sXML &= "                   <cfc:NumeroAbono>" & Query_Manager_H("cfc:NumeroAbono", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cfc:NumeroAbono>"
                        End If

                        If Query_Manager_H("cfc:FechaVencimiento", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            sXML &= "               <cfc:FechaVencimiento>" & CDate(oRecordSetH.Fields.Item("DocDueDate").Value).ToString("yyyy-MM-dd") & "</cfc:FechaVencimiento>"
                        Else
                            sXML &= "                   <cfc:FechaVencimiento>" & Query_Manager_H("cfc:FechaVencimiento", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</cfc:FechaVencimiento>"
                        End If


                        If Query_Manager_H("cfc:MontoAbono", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then

                            If oRecordSetH.Fields.Item("DocCur").Value = "QTZ" Or oRecordSetH.Fields.Item("DocCur").Value = "GTQ" Or oRecordSetH.Fields.Item("DocCur").Value = "Q" Then
                                sXML &= "               <cfc:MontoAbono>" & Convert.ToDecimal(oRecordSetH.Fields.Item("DocTotal").Value).ToString(sDecimal).Replace(",", "") & "</cfc:MontoAbono>"
                            ElseIf oRecordSetH.Fields.Item("DocCur").Value = "USD" Then
                                sXML &= "               <cfc:MontoAbono>" & Convert.ToDecimal(oRecordSetH.Fields.Item("DocTotalFC").Value).ToString(sDecimal).Replace(",", "") & "</cfc:MontoAbono>"
                            End If

                        Else
                            sXML &= "                   <cfc:MontoAbono>" & Convert.ToDecimal(Query_Manager_H("cfc:MontoAbono", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value).ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;")).ToString(sDecimal).Replace(",", "") & "</cfc:MontoAbono>"
                        End If

                        sXML &= "               </cfc:Abono>"
                        sXML &= "           </cfc:AbonosFacturaCambiaria>"
                        sXML &= "     </dte:Complemento>"
                        sXML &= "   </dte:Complementos>"

                    End If

                    If oRecordSetSERIESO.Fields.Item("U_Type").Value = "NCRE" Then
                        'If Not Query_Manager_H("NumeroAutorizacionDocumentoOrigen", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                        sXML &= "   <dte:Complementos>"
                        sXML &= "       <dte:Complemento NombreComplemento=""GT_Complemento_Referencia_Nota"" URIComplemento=""http://www.sat.gob.gt/face2/ComplementoReferenciaNota/0.1.0"">"

                        Dim oRecordSetBuscaCae As SAPbobsCOM.Recordset

                        oRecordSetBuscaCae = Nothing
                        oRecordSetBuscaCae = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "SELECT ""U_CAE"" FROM ORIN "
                        sQuery &= "WHERE ""DocEntry"" = " & oRecordSetH.Fields.Item("DocEntry").Value
                        oRecordSetBuscaCae.DoQuery(sQuery)

                        oRecordSetBuscaDatos = Nothing
                        oRecordSetBuscaDatos = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

                        If Query_Manager_H("NumeroAutorizacionDocumentoOrigen", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            oRecordSetBuscaDatos = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                            sQuery = "Select T1.""DocDate"", T1.""U_CAE"",T1.""DocTotal"" from OINV T1 "
                            sQuery &= "where cast(T1.""U_CAE"" as nvarchar(100)) = '" & oRecordSetBuscaCae.Fields.Item("U_CAE").Value & "'"
                            oRecordSetBuscaDatos.DoQuery(sQuery)
                        Else
                            oRecordSetBuscaDatos = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                            sQuery = "Select T1.""DocDate"", T1.""U_CAE"",T1.""DocTotal"" from OINV T1 "
                            sQuery &= "where cast(T1.""U_CAE"" as nvarchar(100)) = '" & Query_Manager_H("NumeroAutorizacionDocumentoOrigen", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) & "'"
                            oRecordSetBuscaDatos.DoQuery(sQuery)
                        End If

                        oRecordSetFace = Nothing
                        oRecordSetFace = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select ""U_RSGFACE"",""U_NDGFACE"", ""U_SGFACE"", ""U_FeGFACE"" from ORIN where ""DocEntry"" = " & oRecordSetH.Fields.Item("DocEntry").Value
                        oRecordSetFace.DoQuery(sQuery)

                        oRecordRegimenA = Nothing
                        oRecordRegimenA = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select ""U_Antiguo"" from ORIN T1 "
                        sQuery &= "where cast(T1.""U_CAE"" as nvarchar(100)) = '" & oRecordSetBuscaCae.Fields.Item("U_CAE").Value & "'"

                        If oRecordSetH.Fields.Item("U_Antiguo").Value = "Y" Then
                            sXML &= "       <cno:ReferenciasNota NumeroDocumentoOrigen=" & Chr(34) & oRecordSetFace.Fields.Item("U_NDGFACE").Value.ToString() & Chr(34) & " RegimenAntiguo=""Antiguo"" Version=""1""  MotivoAjuste=" & Chr(34) & oRecordSetH.Fields.Item("Comments").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " FechaEmisionDocumentoOrigen=" & Chr(34) & CDate(oRecordSetFace.Fields.Item("U_FeGFACE").Value).ToString("yyyy-MM-dd") & Chr(34) & " SerieDocumentoOrigen=" & Chr(34) & oRecordSetFace.Fields.Item("U_SGFACE").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " NumeroAutorizacionDocumentoOrigen=" & Chr(34) & oRecordSetFace.Fields.Item("U_RSGFACE").Value.ToString & Chr(34) & "></cno:ReferenciasNota>"
                        Else
                            If oRecordSetH.Fields.Item("DocTotal").Value = oRecordSetBuscaDatos.Fields.Item(2).Value Then
                                sXML &= "       <cno:ReferenciasNota FechaEmisionDocumentoOrigen=" & Chr(34) & CDate(oRecordSetBuscaDatos.Fields.Item(0).Value).ToString("yyyy-MM-dd") & Chr(34) & " MotivoAjuste= " & Chr(34) & oRecordSetH.Fields.Item("Comments").Value & Chr(34) & " NumeroAutorizacionDocumentoOrigen=" & Chr(34) & oRecordSetBuscaDatos.Fields.Item(1).Value & Chr(34) & " Version=""1""></cno:ReferenciasNota>"
                            Else
                                sXML &= "       <cno:ReferenciasNota FechaEmisionDocumentoOrigen=" & Chr(34) & CDate(oRecordSetH.Fields.Item("U_FacFecha").Value).ToString("yyyy-MM-dd") & Chr(34) & " MotivoAjuste= " & Chr(34) & oRecordSetH.Fields.Item("Comments").Value & Chr(34) & " NumeroAutorizacionDocumentoOrigen=" & Chr(34) & oRecordSetH.Fields.Item("U_CAE").Value.ToString() & Chr(34) & " Version=""1""></cno:ReferenciasNota>"
                            End If
                        End If


                        sXML &= "       </dte:Complemento>"
                        sXML &= "   </dte:Complementos>"
                    Else

                    End If

                    If oRecordSetSERIESO.Fields.Item("U_Type").Value = "NDEB" Then

                        ' If Not Query_Manager_H("NumeroAutorizacionDocumentoOrigen", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                        sXML &= "   <dte:Complementos>"
                        sXML &= "       <dte:Complemento IDComplemento="""" NombreComplemento="""" URIComplemento="""">"

                        oRecordSetBuscaDatos = Nothing
                        oRecordSetBuscaDatos = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

                        If B1Connections.diCompany.DbServerType = BoDataServerTypes.dst_HANADB Then
                            sQuery = "Select * from OINV T0 Where cast(T0.""U_CAE"" as nvarchar(150)) = '" & Query_Manager_H("NumeroAutorizacionDocumentoOrigen", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) & "' "
                        Else
                            sQuery = "Select * from OINV T0 Where cast(T0.""U_CAE"" as nvarchar(max)) = '" & Query_Manager_H("NumeroAutorizacionDocumentoOrigen", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) & "' "
                        End If

                        Dim oRecordSetBuscaCae As SAPbobsCOM.Recordset

                        oRecordSetBuscaCae = Nothing
                        oRecordSetBuscaCae = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "SELECT ""U_CAE"" FROM OINV "
                        sQuery &= "WHERE ""DocEntry"" = " & oRecordSetH.Fields.Item("DocEntry").Value
                        oRecordSetBuscaCae.DoQuery(sQuery)

                        oRecordSetBuscaDatos = Nothing
                        oRecordSetBuscaDatos = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

                        If Query_Manager_H("NumeroAutorizacionDocumentoOrigen", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) = "" Then
                            oRecordSetBuscaDatos = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                            sQuery = "Select T1.""DocDate"", T1.""U_CAE"",T1.""DocTotal"", T1.""U_DocSerie"", T1.""U_DocNum"" from OINV T1 "
                            sQuery &= "where cast(T1.""U_CAE"" as nvarchar(100)) = '" & oRecordSetBuscaCae.Fields.Item("U_CAE").Value & "'"
                            oRecordSetBuscaDatos.DoQuery(sQuery)
                        Else
                            oRecordSetBuscaDatos = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                            sQuery = "Select T1.""DocDate"", T1.""U_CAE"",T1.""DocTotal"", T1.""U_DocSerie"", T1.""U_DocNum"" from OINV T1 "
                            sQuery &= "where cast(T1.""U_CAE"" as nvarchar(100)) = '" & Query_Manager_H("NumeroAutorizacionDocumentoOrigen", oRecordSetH.Fields.Item("DocEntry").Value, oRecordSetSERIESO.Fields.Item("U_Type").Value) & "'"
                            oRecordSetBuscaDatos.DoQuery(sQuery)
                        End If

                        oRecordSetFace = Nothing
                        oRecordSetFace = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                        sQuery = "Select ""U_RSGFACE"",""U_NDGFACE"", ""U_SGFACE"", ""U_FeGFACE"" from OINV where ""DocEntry"" = " & oRecordSetH.Fields.Item("DocEntry").Value
                        oRecordSetFace.DoQuery(sQuery)

                        If oRecordSetFace.Fields.Item("U_NDGFACE").Value.ToString <> "" Then
                            sXML &= "       <cno:ReferenciasNota xmlns:cno=""http://www.sat.gob.gt/face2/ComplementoReferenciaNota/0.1.0"" NumeroDocumentoOrigen=" & Chr(34) & oRecordSetFace.Fields.Item("U_NDGFACE").Value.ToString() & Chr(34) & " RegimenAntiguo=""Antiguo"" Version=""0.0""  MotivoAjuste=" & Chr(34) & oRecordSetH.Fields.Item("Comments").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " FechaEmisionDocumentoOrigen=" & Chr(34) & CDate(oRecordSetFace.Fields.Item("U_FeGFACE").Value).ToString("yyyy-MM-dd") & Chr(34) & " SerieDocumentoOrigen=" & Chr(34) & oRecordSetFace.Fields.Item("U_SGFACE").Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & Chr(34) & " NumeroAutorizacionDocumentoOrigen=" & Chr(34) & oRecordSetFace.Fields.Item("U_RSGFACE").Value.ToString & Chr(34) & "></cno:ReferenciasNota>"
                        ElseIf oRecordSetH.Fields.Item("DocTotal").Value = oRecordSetBuscaDatos.Fields.Item(2).Value Then

                            sXML &= "       <cno:ReferenciasNota FechaEmisionDocumentoOrigen=" & Chr(34) & CDate(oRecordSetBuscaDatos.Fields.Item(0).Value).ToString("yyyy-MM-dd") & Chr(34) & " MotivoAjuste= " & Chr(34) & oRecordSetH.Fields.Item("Comments").Value & Chr(34) & " NumeroAutorizacionDocumentoOrigen=" & Chr(34) & oRecordSetBuscaDatos.Fields.Item(1).Value & Chr(34) & " SerieDocumentoOrigen=" & Chr(34) & oRecordSetH.Fields.Item("U_DocSerie").Value & Chr(34) & "  Version=""1"" ></cno:ReferenciasNota>"
                        Else
                            sXML &= "       <cno:ReferenciasNota FechaEmisionDocumentoOrigen=" & Chr(34) & CDate(oRecordSetBuscaDatos.Fields.Item(0).Value).ToString("yyyy-MM-dd") & Chr(34) & " MotivoAjuste=" & Chr(34) & oRecordSetH.Fields.Item("Comments").Value & Chr(34) & " NumeroAutorizacionDocumentoOrigen=" & Chr(34) & oRecordSetBuscaDatos.Fields.Item(1).Value & Chr(34) & " SerieDocumentoOrigen=" & Chr(34) & oRecordSetH.Fields.Item("U_DocSerie").Value & Chr(34) & " Version=""1""></cno:ReferenciasNota>"
                        End If
                        sXML &= "       </dte:Complemento>"
                        sXML &= "   </dte:Complementos>"
                    End If

                    sXML &= "      </dte:DatosEmision>"
                    sXML &= "    </dte:DTE>"

                    Dim oRecordSetAdenda As SAPbobsCOM.Recordset

                    oRecordSetAdenda = Nothing
                    oRecordSetAdenda = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    sQuery = "Select * from ""@PERSONAH"" "
                    oRecordSetAdenda.DoQuery(sQuery)

                    If oRecordSetAdenda.RecordCount > 0 Then
                        sXML &= "    <dte:Personalizado>"
                        For iContAdenda As Integer = 0 To oRecordSetAdenda.RecordCount - 1
                            If oRecordSetAdenda.Fields.Item("U_Field").Value = "GroupNum" Then

                                oRecordSetPago = Nothing
                                oRecordSetPago = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                                sQuery = "Select T0.""PymntGroup"" from OCTG T0 Where T0.""GroupNum"" = " & oRecordSetH.Fields.Item("GroupNum").Value
                                oRecordSetPago.DoQuery(sQuery)
                                oRecordSetPago.MoveFirst()

                                sXML &= "       <" & oRecordSetAdenda.Fields.Item("U_Adenda").Value & ">" & oRecordSetPago.Fields.Item(0).Value.Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</" & oRecordSetAdenda.Fields.Item("U_Adenda").Value & ">"

                            ElseIf oRecordSetAdenda.Fields.Item("U_Field").Value = "SlpCode" Then
                                oRecordSetPago = Nothing
                                oRecordSetPago = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                                sQuery = "Select T0.""SlpName"" from OSLP T0 Where T0.""SlpCode"" = " & oRecordSetH.Fields.Item("SlpCode").Value
                                oRecordSetPago.DoQuery(sQuery)
                                oRecordSetPago.MoveFirst()

                                sXML &= "       <" & oRecordSetAdenda.Fields.Item("U_Adenda").Value & ">" & oRecordSetPago.Fields.Item(0).Value.Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</" & oRecordSetAdenda.Fields.Item("U_Adenda").Value & ">"
                            Else
                                If oRecordSetAdenda.Fields.Item("U_Type").Value = "T" Then
                                    sXML &= "       <" & oRecordSetAdenda.Fields.Item("U_Adenda").Value & ">" & oRecordSetH.Fields.Item(oRecordSetAdenda.Fields.Item("U_Field").Value).Value.ToString().Replace("'", "&apos;").Replace("""\""", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;") & "</" & oRecordSetAdenda.Fields.Item("U_Adenda").Value & ">"
                                ElseIf oRecordSetAdenda.Fields.Item("U_Type").Value = "D" Then
                                    sXML &= "       <" & oRecordSetAdenda.Fields.Item("U_Adenda").Value & ">" & Convert.ToDecimal(SumaIva).ToString(oRecordSetH.Fields.Item(oRecordSetAdenda.Fields.Item("U_Field").Value).Value).Replace(",", "") & "</" & oRecordSetAdenda.Fields.Item("U_Adenda").Value & ">"
                                ElseIf oRecordSetAdenda.Fields.Item("U_Type").Value = "E" Then
                                    sXML &= "       <" & oRecordSetAdenda.Fields.Item("U_Adenda").Value & ">" & Integer.Parse(oRecordSetH.Fields.Item(oRecordSetAdenda.Fields.Item("U_Field").Value).Value) & "</" & oRecordSetAdenda.Fields.Item("U_Adenda").Value & ">"
                                ElseIf oRecordSetAdenda.Fields.Item("U_Type").Value = "F" Then
                                    CDate(oRecordSetH.Fields.Item(oRecordSetAdenda.Fields.Item("U_Field").Value).Value).ToString(oRecordSetAdenda.Fields.Item("U_Format").Value)
                                End If
                            End If

                            oRecordSetAdenda.MoveNext()
                        Next
                        sXML &= "     </dte:Personalizado>"
                    End If

                    sXML &= "  </dte:SAT>"
                    sXML &= "</dte:GTDocumento>"

                    oArchivo.WriteLine(sXML)
                    oArchivo.Flush()
                    oArchivo.Close()

                    UrlEnvio = oRecordSetFEL.Fields.Item("U_UrlFel").Value
                    TipoEnvio = oRecordSetSERIESO.Fields.Item("U_Type").Value
                    AreaCliente = oRecordSetFEL.Fields.Item("U_AreaName").Value

                    Dim Tipo As String = oRecordSetSERIESO.Fields.Item("U_Type").Value = "CAXP"

                    If TipoEnvio = "CAXP" Then
                        TipoEnvio = "FCAM"
                    End If

                    PostXML(sXML, TipoEnvio, TipoDoc, AreaCliente, UrlEnvio)

                End If
            End If
        Catch ex As Exception
            B1Connections.theAppl.MessageBox(ex.Message(), BoMessageTime.bmt_Medium)
        End Try
    End Sub

    Public Function PostXML(v_XML_String As String, sTipo As String, SDocTypeSAP As String, sArea As String, sUrlws As String) As XmlDocument
        Try

            Dim v_XML_Res As XmlDocument = New XmlDocument
            Dim v_XML_Out As XmlDocument = New XmlDocument
            Dim xmlNodeRdrOut As XmlNodeReader
            Dim XmlOut As String = ""
            Dim XmlOutRes As String = ""
            Dim sRuta As String = ""
            ' Dim MensajeRespuesta, Numero, Serie As String
            Dim nMen As String
            Dim nVal As String
            Dim nRes As String
            Dim nXml As String


            Dim respuesta As New wsFirma.Core

            If oRecordSetFEL.RecordCount = 0 Then
                B1Connections.theAppl.MessageBox("No Existe Ruta Para Depositar Los Archivos..")
                Exit Try
            End If
            If SDocTypeSAP = "13" Then
                If System.IO.File.Exists(oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\Factura_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml") Then
                    'The file exists
                    System.IO.File.Delete(oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\Factura_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml")
                End If
                oArchivo2 = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\Factura_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml")
                sRuta = oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\Factura_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml"

            ElseIf SDocTypeSAP = "18" Then
                If System.IO.File.Exists(oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\FacturaEspecial_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml") Then
                    'The file exists
                    System.IO.File.Delete(oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\FacturaEspecial_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml")
                End If
                oArchivo = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\FacturaEspecial_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml")
                sRuta = oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\FacturaEspecial_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml"

            ElseIf (SDocTypeSAP = "14") Then
                If System.IO.File.Exists(oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\NotaCredito_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml") Then
                    'The file exists
                    System.IO.File.Delete(oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\NotaCredito_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml")
                End If
                oArchivo2 = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\NotaCredito_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml")
                sRuta = oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\NotaCredito_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml"

            ElseIf (SDocTypeSAP = "DN") Then
                If System.IO.File.Exists(oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\NotaDebito_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml") Then
                    System.IO.File.Delete(oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\NotaDebito_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml")
                End If
                oArchivo2 = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\NotaDebito_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml")
                sRuta = oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\NotaDebito_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml"

            ElseIf (SDocTypeSAP = "24") Then
                oArchivo2 = File.AppendText(oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\Recibo_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml")
                sRuta = oRecordSetFEL.Fields.Item("U_RutaRes").Value & "\Recibo_" & oRecordSetH.Fields.Item("DocNum").Value & "_RESP1.xml"

            End If

            Dim errorWs As String

            'Variable de retorno de XML 
            Dim v_XML_Return As String  'String = String.Empty
            Dim v_WS_Pwd As String = "administrador"

            'Variable String para trabajar localmente el parseo
            Dim v_WS_String As String
            'Dim wsReturn = New wsFirmaFel.CoreSoapClient
            ' MsgBox(sUrlws)
            respuesta.Url = sUrlws
            Dim envio = respuesta.ConvertSignDocumentWithConnector(sArea, "administrador", sTipo, sXML, "Xslt")

            v_WS_String = envio.ToString
            v_WS_String = v_WS_String.Replace("&", "&amp;")

            Dim v_XML_Result As XmlDocument = New XmlDocument
            v_XML_Result.LoadXml(v_WS_String)


            xmlNodeRdr = New XmlNodeReader(v_XML_Result)
            dsGenerarGuia = New DataSet()
            dsGenerarGuia.ReadXml(xmlNodeRdr)

            If dsGenerarGuia.Tables.Count > 0 Then
                nVal = dsGenerarGuia.Tables(0).Rows(0)("HasError").ToString()

                If nVal = "true" Then
                    nXml = dsGenerarGuia.Tables.Item("Error").Rows.Item(0).Item(1).ToString()



                Else
                    nXml = dsGenerarGuia.Tables(0).Rows(0)("TextData").ToString()

                    'Proceso de lectura para almacenamiento de XML respuesta
                    v_XML_Out.LoadXml(nXml)
                    xmlNodeRdrOut = New XmlNodeReader(v_XML_Out)
                    dsGenerarOUT = New DataSet()
                    dsGenerarOUT.ReadXml(xmlNodeRdrOut)

                    If dsGenerarOUT.Tables.Count > 0 Then
                        If nVal = "false" Then
                            XmlOut = dsGenerarOUT.Tables.Item("DTECertified").Rows.Item(0).Item(0).ToString()
                        End If
                    End If

                    XmlOutRes = XmlOut.Replace("&lt;", "<")
                    XmlOutRes = XmlOutRes.Replace("&gt;", ">")


                    Texto = XmlOutRes

                    Numero = Texto.LastIndexOf("Numero=")
                    NumeroR = Texto.Substring(Numero, 18)

                    Serie = Texto.LastIndexOf("Serie=")
                    serieR = Texto.Substring(Serie, 17)


                    Numero = NumeroR.Replace("Numero=", "")
                    Numero = Numero.Replace("""", "")
                    Numero = Numero.Replace(">", "")

                    Serie = serieR.Replace("Serie=", "")
                    Serie = Serie.Replace("""", "")

                    If nVal = "false" Then

                        v_XML_Res.LoadXml(nXml)
                        xmlNodeRdr2 = New XmlNodeReader(v_XML_Res)
                        dsGenerarGuia2 = New DataSet()
                        dsGenerarGuia2.ReadXml(xmlNodeRdr2)

                        If dsGenerarGuia2.Tables.Count > 0 Then
                            If nVal = "false" Then
                                nRes = dsGenerarGuia2.Tables.Item("DTECertified").Rows.Item(0).Item(0).ToString()
                                NIT = dsGenerarGuia2.Tables.Item("DTECertified").Rows.Item(0).Item(1).ToString()
                                Certificador = dsGenerarGuia2.Tables.Item("DTECertified").Rows.Item(0).Item(2).ToString()
                                numeroAutorizacion = dsGenerarGuia2.Tables.Item("DTECertified").Rows.Item(0).Item(3).ToString()
                                TimeStamp = dsGenerarGuia2.Tables.Item("DTECertified").Rows.Item(0).Item(4).ToString()

                            End If
                        End If
                    Else
                        nMen = dsGenerarGuia.Tables.Item("Error").Rows.Item(0).Item(1).ToString()
                    End If

                End If

                'v_WS_String = nVal.Replace("&amp;lt;", "<")
                v_WS_String = v_WS_String.Replace("&amp;gt;", ">")
            Else
                numeroAutorizacion = ""
                Serie = ""
                PreImpreso = ""
                Documento = ""
                nombre = ""
                Direccion = ""
                nMen = "Documento Contingencia"
                nVal = "0"
            End If

            oDocumento = Nothing

            If SDocTypeSAP = "13" Or SDocTypeSAP = "20" Or SDocTypeSAP = "12" Or SDocTypeSAP = "DN" Then
                oDocumento = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.oInvoices)

            ElseIf SDocTypeSAP = "14" Or SDocTypeSAP = "21" Then
                oDocumento = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.oCreditNotes)
            ElseIf SDocTypeSAP = "18" Then

                Dim oFactura As SAPbobsCOM.Documents

                oFactura = Nothing
                oFactura = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.oPurchaseInvoices)


                If oFactura.GetByKey(oRecordSetH.Fields.Item("DocEntry").Value) Then


                    If nVal = "false" Then


                        oFactura.UserFields.Fields.Item("U_DocNum").Value = Numero
                        oFactura.UserFields.Fields.Item("U_DocSerie").Value = Serie
                        oFactura.UserFields.Fields.Item("U_CAE").Value = numeroAutorizacion
                        oFactura.UserFields.Fields.Item("U_Fac_FechaC").Value = TimeStamp
                        oFactura.UserFields.Fields.Item("U_Mensaje").Value = "Documento certificado exitosamente"


                        If nVal = "false" Then
                            oFactura.UserFields.Fields.Item("U_Valido").Value = "true"
                        Else
                            oFactura.UserFields.Fields.Item("U_Valido").Value = "false"
                        End If

                        iValida = oFactura.Update()
                        B1Connections.theAppl.MessageBox("Documento certificado exitosamente")

                        If iValida <> 0 Then
                            B1Connections.diCompany.GetLastError(lErrCode, sErrMsg)
                            B1Connections.theAppl.MessageBox("ERROR: " & sErrMsg)
                        End If
                    Else
                        oFactura.UserFields.Fields.Item("U_Valido").Value = "false"
                        oFactura.UserFields.Fields.Item("U_Mensaje").Value = dsGenerarGuia.Tables.Item("Error").Rows.Item(0).Item(1).ToString()
                        iValida = oFactura.Update()

                        B1Connections.theAppl.MessageBox("El documento fue rechazado debido a: " & dsGenerarGuia.Tables.Item("Error").Rows.Item(0).Item(1).ToString())
                    End If

                End If

            End If

            If oDocumento.GetByKey(sDocEntry) Then
                If nVal = "false" Then
                    ' oDocumento.UserFields.Fields.Item("U_FacNit").Value = NIT
                    'oDocumento.UserFields.Fields.Item("U_DocNom").Value = Certificador
                    oDocumento.UserFields.Fields.Item("U_DocNum").Value = Numero
                    oDocumento.UserFields.Fields.Item("U_DocSerie").Value = Serie
                    oDocumento.UserFields.Fields.Item("U_CAE").Value = numeroAutorizacion
                    oDocumento.UserFields.Fields.Item("U_FacFecha").Value = TimeStamp
                    oDocumento.UserFields.Fields.Item("U_Fac_FechaC").Value = TimeStamp
                    oDocumento.UserFields.Fields.Item("U_Mensaje").Value = "Documento certificado exitosamente"



                    If nVal = "false" Then
                        oDocumento.UserFields.Fields.Item("U_Valido").Value = "true"
                    Else
                        oDocumento.UserFields.Fields.Item("U_Valido").Value = "false"
                    End If

                    iValida = oDocumento.Update()
                    B1Connections.theAppl.MessageBox("Documento certificado exitosamente")

                    If iValida <> 0 Then
                        B1Connections.diCompany.GetLastError(lErrCode, sErrMsg)
                        B1Connections.theAppl.MessageBox("ERROR: " & sErrMsg)
                    End If
                Else
                    oDocumento.UserFields.Fields.Item("U_Valido").Value = "false"
                    oDocumento.UserFields.Fields.Item("U_Mensaje").Value = dsGenerarGuia.Tables.Item("Error").Rows.Item(0).Item(1).ToString()
                    iValida = oDocumento.Update()

                    B1Connections.theAppl.MessageBox("El documento fue rechazado debido a: " & dsGenerarGuia.Tables.Item("Error").Rows.Item(0).Item(1).ToString())
                End If

                oArchivo2.WriteLine(XmlOutRes)
                oArchivo2.Flush()
                oArchivo2.Close()

            End If

        Catch ex As Exception
            B1Connections.theAppl.MessageBox(ex.Message)
        End Try
    End Function

    Public Function PostXMLAnul(v_XML_String As String, sTipo As String, SDocTypeSAP As String, sArea As String) As XmlDocument
        Try
            'Variable de retorno de XML 
            Dim v_XML_Return As String  'String = String.Empty
            Dim v_WS_Pwd As String = "administrador"

            'Variable String para trabajar localmente el parseo
            Dim v_WS_String As String
            Dim wsReturn = New wsFirma.Core
            v_XML_Return = wsReturn.ConvertSignDocumentWithConnector(sArea, v_WS_Pwd, sTipo, v_XML_String, "Xslt")

            v_WS_String = v_XML_Return.ToString
            v_WS_String = v_WS_String.Replace("&", "&amp;")

            Dim v_XML_Result As XmlDocument = New XmlDocument
            v_XML_Result.LoadXml(v_WS_String)

            Dim nMen As String
            Dim nVal As String
            Dim nRes As String
            Dim nXml As String

            xmlNodeRdr = New XmlNodeReader(v_XML_Result)
            dsGenerarGuia = New DataSet()
            dsGenerarGuia.ReadXml(xmlNodeRdr)

            If dsGenerarGuia.Tables.Count > 0 Then
                nVal = dsGenerarGuia.Tables(0).Rows(0)("HasError").ToString()
                nXml = dsGenerarGuia.Tables(0).Rows(0)("TextData").ToString()

                v_WS_String = nVal.Replace("&amp;lt;", "<")
                v_WS_String = v_WS_String.Replace("&amp;gt;", ">")

                If nVal = "false" Then
                    Dim v_XML_Res As XmlDocument = New XmlDocument
                    v_XML_Res.LoadXml(nXml)
                    xmlNodeRdr2 = New XmlNodeReader(v_XML_Res)
                    dsGenerarGuia2 = New DataSet()
                    dsGenerarGuia2.ReadXml(xmlNodeRdr2)

                    If dsGenerarGuia2.Tables.Count > 0 Then
                        If nVal = "false" Then
                            nRes = dsGenerarGuia2.Tables.Item("DTECertified").Rows.Item(0).Item(0).ToString()
                            NIT = dsGenerarGuia2.Tables.Item("DTECertified").Rows.Item(0).Item(1).ToString()
                            Certificador = dsGenerarGuia2.Tables.Item("DTECertified").Rows.Item(0).Item(2).ToString()
                            numeroAutorizacion = dsGenerarGuia2.Tables.Item("DTECertified").Rows.Item(0).Item(3).ToString()
                            TimeStamp = dsGenerarGuia2.Tables.Item("DTECertified").Rows.Item(0).Item(4).ToString()
                        Else
                            nMen = dsGenerarGuia.Tables.Item("Error").Rows.Item(0).Item(1).ToString()
                        End If
                    End If
                Else
                    nMen = dsGenerarGuia.Tables.Item("Error").Rows.Item(0).Item(1).ToString()
                End If
            Else
                numeroAutorizacion = ""
                Serie = ""
                PreImpreso = ""
                Documento = ""
                nombre = ""
                Direccion = ""
                nMen = "Documento Contingencia"
                nVal = "0"
            End If

            oDocumento = Nothing

            If SDocTypeSAP = "13" Or SDocTypeSAP = "20" Or SDocTypeSAP = "exp" Or SDocTypeSAP = "12" Then
                oDocumento = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.oInvoices)

            ElseIf SDocTypeSAP = "14" Or SDocTypeSAP = "21" Then
                oDocumento = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.oCreditNotes)
            End If

            If oDocumento.GetByKey(sDocEntry) Then
                If nVal = "false" Then
                    'oDocumento.UserFields.Fields.Item("U_FacNit").Value = NIT
                    'oDocumento.UserFields.Fields.Item("U_DocNom").Value = Certificador
                    oDocumento.UserFields.Fields.Item("U_CAE").Value = numeroAutorizacion
                    oDocumento.UserFields.Fields.Item("U_FacFecha").Value = TimeStamp
                    oDocumento.UserFields.Fields.Item("U_Fac_FechaC").Value = TimeStamp

                    If nVal = "false" Then
                        oDocumento.UserFields.Fields.Item("U_Valido").Value = "true"
                    Else
                        oDocumento.UserFields.Fields.Item("U_Valido").Value = "false"
                    End If

                    iValida = oDocumento.Update()
                    B1Connections.theAppl.MessageBox("Documento certificado exitosamente")

                    If iValida <> 0 Then
                        B1Connections.diCompany.GetLastError(lErrCode, sErrMsg)
                        B1Connections.theAppl.MessageBox("ERROR: " & sErrMsg)
                    End If
                Else
                    B1Connections.theAppl.MessageBox("El documento fue rechazado debido a: " & dsGenerarGuia.Tables.Item("Error").Rows.Item(0).Item(1).ToString())
                End If
            End If
        Catch ex As Exception
            B1Connections.theAppl.MessageBox(ex.Message)
        End Try
    End Function

    Sub LeerXML(sRuta As String)

        Dim m_xmlr As XmlTextReader
        m_xmlr = New XmlTextReader(sRuta)

        m_xmlr.WhitespaceHandling = WhitespaceHandling.None

        'Se recorre para llegar a la posicion del receptor
        Dim i As Integer

        '25 Es el numero de nodos antes del nodo receptor
        While i <= 104
            m_xmlr.Read()
            i += 1
        End While

        'Dim NITR = m_xmlr.GetAttribute("IDReceptor")
        Dim sSerie As String
        Dim sNum As String

        sSerie = m_xmlr.GetAttribute("Serie")
        sNum = m_xmlr.GetAttribute("Numero")

        'Cerramos la lectura del archivo
        m_xmlr.Close()

        'Conexion a bd y actualización de datos
        oRecordSetUpdateNum = Nothing
        oRecordSetUpdateNum = B1Connections.diCompany.GetBusinessObject(BoObjectTypes.BoRecordset)

        sQuery = ""
        sQuery = "Update OINV "

        ' sQuery &= " set ""U_DocNum"" = '" & NombreReceptor & "'"
        sQuery &= " where ""DocEntry"" = " & oRecordSetH.Fields.Item("DocEntry").Value

        oRecordSetUpdateNum.DoQuery(sQuery)
    End Sub
    Public Function Desencriptar(ByVal Input As String) As String
        Try
            Dim IV() As Byte = ASCIIEncoding.ASCII.GetBytes("S0luti0n") 'La clave debe ser de 8 caracteres
            Dim EncryptionKey() As Byte = Convert.FromBase64String("irpSIvNmJKlrzcmtPU9/c89Gkj7yL1S7") 'No se puede alterar la cantidad de caracteres pero si la clave
            Dim buffer() As Byte = Convert.FromBase64String(Input)
            Dim des As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider
            des.Key = EncryptionKey
            des.IV = IV
            Return Encoding.UTF8.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length()))
        Catch ex As Exception
            B1Connections.theAppl.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Medium, True)
        End Try
    End Function
    Public Function quitarSaltosLinea(ByVal texto As String,
                     caracterReemplazar As String) As String
        quitarSaltosLinea = Replace(Replace(texto, Chr(10),
            caracterReemplazar), Chr(13), caracterReemplazar)
    End Function


End Module
