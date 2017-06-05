<AttributeUsage(AttributeTargets.Method)>
Public Class CommandAttribute
    Inherits Attribute

#Region "Method Property"
    Private _Method As Reflection.MethodInfo

    Public Property Method As Reflection.MethodInfo
        Get
            Return Me._Method
        End Get
        Set(ByVal Value As Reflection.MethodInfo)
            Me._Method = Value
        End Set
    End Property
#End Region

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

#Region "ParametersDescriptions Property"
    Private _ParametersDescriptions As ParameterDescriptionAttribute()

    Public Property ParametersDescriptions As ParameterDescriptionAttribute()
        Get
            Return Me._ParametersDescriptions
        End Get
        Set(ByVal Value As ParameterDescriptionAttribute())
            Me._ParametersDescriptions = Value
        End Set
    End Property
#End Region

End Class
