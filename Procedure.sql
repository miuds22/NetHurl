-- Procedure.sql
drop table if EXISTS [NET_Equipos] , [NET_EquipoStatus] 
USE [InfoPartidos3]
GO

/****** Object:  Table [dbo].[NET_Equipos]    Script Date: 07/08/2024 13:09:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NET_Equipos](
	[cdEquipo] [int] IDENTITY(1,1) NOT NULL,
	[dsInventario] [varchar](512) NULL,
	[cdsede] [int] NULL,
	[dsSector] [varchar](200) NULL,
	[dsInventario_Monitor] [varchar](512) NULL,
	[dsdescripcion] [varchar](512) NULL,
	[dsClave_activacion] [varchar](512) NULL,
	[dsMAC] [varchar](512) NULL,
	[dsMacGW] [varchar](512) NULL,
	[cdUsuario] [varchar](512) NULL,
	[icActivo] [tinyint] NULL,
 CONSTRAINT [PK_NET_Equipos_1] PRIMARY KEY CLUSTERED 
(
	[cdEquipo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
USE [InfoPartidos3]
GO

/****** Object:  Table [dbo].[NET_EquipoStatus]    Script Date: 07/08/2024 13:09:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[NET_EquipoStatus](
	[cdEquipoStatus] [int] NULL,
	[vlCantidadDiscos] [int] NULL,
	[jsAppsInstaladas] [varchar](max) NULL,
	[vlespacioDisco] int NULL,
	[vlespacioLibre] int NULL,
	[dsIP_Publica] [varchar](512) NULL,
	[dsIP_Privada] [varchar](512) NULL,
	[vlLatencia] [varchar](512) NULL,
	[dsSO] [varchar](512) NULL,
	[dtStatus] [datetime] NOT NULL,
	[cdEquipo] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[NET_EquipoStatus] ADD  CONSTRAINT [DF_NET_EquipoStatus_dtStatus]  DEFAULT (dateadd(hour,(-3),getutcdate())) FOR [dtStatus]
GO




/*
CREATE TABLE NET_Nodos (
    cdNodo	INT,
    dsMAC	VARCHAR(512),
    cdEmpresa INT,
    ip_publica	VARCHAR(512),
    idtipoEnlace   INT
);

CREATE TABLE NET_Sedes (
    cdsede int,
    dsdescripcion varchar(200),
    dsDireccion varchar(200)
);

CREATE TABLE NET_Empresas (
    cdEmpresa INT,
    dsNombre	VARCHAR(512)
);



CREATE TABLE NET_TiposEnlaces (
    idtipoEnlace   INT,
    dsanchoBanda varchar(200),
    icSimetrico int
);

*/


declare @jsParametro varchar(max) = '{ "nro_inventario": "158967555", "Sector": "Compras","Sede": "2", "nro_inventario_Monitor": "9865877852", "descripcion": "Impresora 3D - Marca Manaos", "Clave_activacion": "asdas-fasdh46-324-ysdfas34-ghb-1112", "cant_discos": "7", "aps_instaladas": "{algo}", "espacio_disco": "15665465", "espacio_libre": "15555", "MAC": "9061AEDC5276", "ip_publica": "180.186.122.12", "ip_privada": "192.168.100.1", "latencia": "45", "SistemaOperativo": "Unix 6.8.11.64", "usuario": "mrk" }'

declare @nro_inventario varchar(255) = ''
declare @Sector varchar(255) = ''
declare @Sede varchar(255)=''
declare @nro_inventario_Monitor varchar(255) = ''
declare @descripcion varchar(255) = ''
declare @idEnlace varchar(255) = ''
declare @Clave_activacion varchar(255) = ''
declare @cant_discos varchar(255) = ''
declare @aps_instaladas varchar(255) = ''
declare @espacio_disco varchar(255) = ''
declare @espacio_libre varchar(255) = ''
declare @MAC varchar(255) = ''
declare @ip_publica varchar(255) = ''
declare @ip_privada varchar(255) = ''
declare @latencia varchar(255) = ''
declare @SistemaOperativo varchar(255) = ''
declare @usuario varchar(255) = ''
-------------
declare @cdEquipo int;


select
    @nro_inventario = nro_inventario,
    @Sector = Sector,
    @Sede = Sede,
    @nro_inventario_Monitor = nro_inventario_Monitor,
    @descripcion = descripcion,   
    @Clave_activacion = Clave_activacion,
    @cant_discos = cant_discos,
    @aps_instaladas = aps_instaladas,
    @espacio_disco = espacio_disco,
    @espacio_libre = espacio_libre,
    @MAC = MAC,
    @ip_publica = ip_publica,
    @ip_privada = ip_privada,
    @latencia = latencia,
    @SistemaOperativo = SistemaOperativo,
    @usuario = usuario
from
 openjson(@jsParametro)
		  with (
                nro_inventario varchar(255) '$.nro_inventario'
                ,Sector varchar(255) '$.Sector'
                ,Sede varchar(255) '$.Sede'
                ,nro_inventario_Monitor varchar(255) '$.nro_inventario_Monitor'
                ,descripcion varchar(255) '$.descripcion'
   
				,Clave_activacion varchar(255) '$.Clave_activacion'
                ,cant_discos varchar(255) '$.cant_discos'
                ,aps_instaladas varchar(255) '$.aps_instaladas'
                ,espacio_disco varchar(255) '$.espacio_disco'
                ,espacio_libre varchar(255) '$.espacio_libre'
                ,MAC varchar(255) '$.MAC'
                ,ip_publica varchar(255) '$.ip_publica'
                ,ip_privada varchar(255) '$.ip_privada'
                ,latencia varchar(255) '$.latencia'
                ,SistemaOperativo varchar(255) '$.SistemaOperativo'
                ,usuario varchar(255) '$.usuario'
                )

				
--leemos si no existe
select @cdEquipo =  isnull(dsinventario ,0) from NET_Equipos where dsInventario = @nro_inventario

-- >>Persistimos NET_Equipos
if @cdEquipo is null 
begin
	INSERT INTO [dbo].[NET_Equipos]
			   ([dsInventario]
			   ,[cdsede]
			   ,[dsSector]
			   ,[dsInventario_Monitor]
			   ,[dsdescripcion]
			   ,[dsClave_activacion]
			   ,[dsMAC]
			   ,[dsMacGW]
			   ,[cdUsuario]
			   ,[icActivo])
		 VALUES
	(@nro_inventario,@Sede, @Sector, @nro_inventario_Monitor ,@descripcion ,@Clave_activacion ,@MAC ,'',@usuario,1 ) ;

	select @cdEquipo = @@IDENTITY
end --if @cdEquipo > 0 then
-- Persistimos NET_EquipoStatus

insert into  [dbo].[NET_EquipoStatus] (cdEquipo,vlCantidadDiscos,jsAppsInstaladas,vlespacioDisco,vlespacioLibre,dsIP_Publica,dsIP_Privada,vlLatencia,dsSO,dtStatus)
values (@cdEquipo,@cant_discos,@aps_instaladas,@espacio_disco,@espacio_libre,@ip_publica,@ip_privada,@latencia,@SistemaOperativo,GETDATE())


