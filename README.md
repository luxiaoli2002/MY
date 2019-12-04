# MY
# 获取表字段
SELECT
           [number]=a.colorder,
           [column] =a.name,
           [datatype]=b.name,
           [length]=COLUMNPROPERTY(a.id,a.name,'PRECISION'),
           [identity]=case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then '√'else '' end,
           [key]=case when exists(SELECT 1 FROM sysobjects where xtype='PK' and parent_obj=a.id and name in (
     SELECT name FROM sysindexes WHERE indid in(
     SELECT indid FROM sysindexkeys WHERE id = a.id AND colid=a.colid
     ))) then '√' else '' end,
     [isnullable]=case when a.isnullable=1 then '√'else '' end,
     [default]=isnull(e.text,''),
     [remark]=isnull(g.[value],a.name)
     FROM syscolumns a
     left join systypes b on a.xusertype=b.xusertype
     inner join sysobjects d on a.id=d.id  and d.xtype='U' and  d.name<>'dtproperties'
     left join syscomments e on a.cdefault=e.id
     left join sys.extended_properties g on a.id=g.major_id and a.colid=g.minor_id 
     left join sys.extended_properties f on d.id=f.major_id and f.minor_id=0

   where  d.name='AbpRoles' order by a.id,a.colorder
   
   # 获取表结构
   
   DECLARE @TableInfo TABLE
                            (
                              name VARCHAR(50) ,
                              sumrows VARCHAR(11) ,
                              reserved VARCHAR(50) ,
                              data VARCHAR(50) ,
                              index_size VARCHAR(50) ,
                              unused VARCHAR(50) ,
                              pk VARCHAR(50)
                            )
                        DECLARE @TableName TABLE ( name VARCHAR(50) )
                        DECLARE @name VARCHAR(50)
                        DECLARE @pk VARCHAR(50)
                        INSERT  INTO @TableName
                                ( name
                                )
                                SELECT  o.name
                                FROM    sysobjects o ,
                                        sysindexes i
                                WHERE   o.id = i.id
                                        AND o.Xtype = 'U'
                                        AND i.indid < 2
                                ORDER BY i.rows DESC ,
                                        o.name
                        WHILE EXISTS ( SELECT   1
                                       FROM     @TableName ) 
                            BEGIN
                                SELECT TOP 1
                                        @name = name
                                FROM    @TableName 
                                DELETE  @TableName
                                WHERE   name = @name
                                DECLARE @objectid INT
                                SET @objectid = OBJECT_ID(@name)
                                SELECT  @pk = COL_NAME(@objectid, colid)
                                FROM    sysobjects AS o
                                        INNER JOIN sysindexes AS i ON i.name = o.name
                                        INNER JOIN sysindexkeys AS k ON k.indid = i.indid
                                WHERE   o.xtype = 'PK'
                                        AND parent_obj = @objectid
                                        AND k.id = @objectid
                                INSERT  INTO @TableInfo
                                        ( name ,
                                          sumrows ,
                                          reserved ,
                                          data ,
                                          index_size ,
                                          unused                         
                                        )
                                        EXEC sys.sp_spaceused @name
                                UPDATE  @TableInfo
                                SET     pk = @pk
                                WHERE   name = @name
                            END   
                                   
                        SELECT  F.name ,
                                F.reserved ,
                                F.data ,
                                F.index_size ,
                                RTRIM(F.sumrows) AS sumrows ,
                                F.unused ,
                                ISNULL(p.tdescription,f.name) AS tdescription,
                                F.pk
                        FROM    @TableInfo F
                                LEFT JOIN ( SELECT  name = CASE WHEN A.COLORDER = 1 THEN D.NAME
                                                                ELSE ''
                                                           END ,
                                                    tdescription = CASE WHEN A.COLORDER = 1
                                                                        THEN ISNULL(F.VALUE, '')
                                                                        ELSE ''
                                                                   END
                                            FROM    SYSCOLUMNS A
                                                    LEFT JOIN SYSTYPES B ON A.XUSERTYPE = B.XUSERTYPE
                                                    INNER JOIN SYSOBJECTS D ON A.ID = D.ID
                                                                               AND D.XTYPE = 'U'
                                                                               AND D.NAME <> 'DTPROPERTIES'
                                                    LEFT JOIN sys.extended_properties F ON D.ID = F.major_id
                                            WHERE   a.COLORDER = 1
                                                    AND F.minor_id = 0
                                          ) P ON F.name = p.name WHERE   1 = 1 AND CAST(F.name AS VARCHAR(100)) like '%AbpRoles%'  or CAST(p.tdescription  AS VARCHAR(100)) like '%AbpRoles%'
