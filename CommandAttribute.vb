<AttributeUsage(AttributeTargets.Method)>
Public Class CommandAttribute
    Inherits Attribute

#Region "Description Property"
    Private _Description As String

    Public Property Description As String
        Get
            Return Me._Description
        End Get
        Set(ByVal Value As String)
            Me._Description = Value
        End Set
    End Property
#End Region

End Class
