SkyGroundLabs
=============

James custom framework

#SkygroundLabs.Data.Sql Documentation

-Mapping <br/>
1. Column (Target: Field or Property)<br/>
2. DbGenerationOption (Target: Field or Property)<br/>
3. Key (Target: Field or Property)<br/>
4. Table (Target: Class)<br/>
5. Unmapped (Target: Field or Property)<br/>

-Examples<br/>
####Column<br/>
     [Column("Test")]
     public string Testing { get; set; }

<i>Description:</i>  Use if you want to rename the Database Column Name.  "Test" refers to the Database Column Name and "Testing" is the new property name

####DbGenerationOption<br/>
-Options (Generate,IdentitySpecification,None)<br/>
    [DbGenerationOption(DbGenerationType.Generate)]
    public Guid ID { get; set; }

<i>Description:</i>  The first property that is a primary key will automatically use Identity Specification, no attribute is needed.  Any secondary keys or after that need the DbGenerationOption set.  If the first property that is a primary key does not use Identity Specification then the DbGenerationOption must be set to Generate.  This attribute can also be used for other columns that need ID's generated that are not primary keys.  The Generate option will work with numbers (Int16,Int32,Int64) and uniqueidentifiers (Guid).

####Key<br/>

    [Key]
    public Guid ID { get; set; }
    [Key]
    public int PersonID { get; set; }
	
<i>Description:</i>  If one primary key is used then this attribute is not needed, if two or more primary keys are used then this attribute must be set on EACH primary key property.  If this attribute is not used the insert and find functions will not work properly.
	
####Table <br/>

    [Table("TEMP_Test3")]
    public class TEMP_Test
    {
        public Guid ID { get; set; }
        
        [Column("Test")]
        public string Testing { get; set; }
    }

<i>Description:</i>  This attribute only needs to be used when the class name and the table name are different.  In the above example my database table name is TEMP_Test3, but I am renaming it in code to TEMP_Test

####Unmapped <br/>

    [Table("TEMP_Test3")]
    public class TEMP_Test
    {
        public Guid ID { get; set; }
        
        [Column("Test")]
        public string Testing { get; set; }
       
        [Unmapped]
        public string DoNotMapThisColumn { get; set; }
    }

<i>Description:</i>  This attribute only needs to be used when you do not want the column to be mapped to the database.  When its not mapped this column will not be saved/updated or retreived from the database.

##Returning Data <br/>

####Find<br/>

    var next = context.Find<TEMP_Test>(new Guid("A73D317B-273C-48E3-B963-953FDFEC6D89"), 1);
    
<i>Description:</i>  In the above example the find function is looking for a matching Guid and Integer.  Note that the keys must be put into the find function in the order they appear in the class.  TEMP_Test is my return class type.

####Select<br/>

    while (context.HasNext())
    {
        var item = context.Select<TEMP_Test>();
    }
    
<i>Description:</i>  Select is meant to be used with HasNext() to loop through results.

####First<br/>

    var item = context.First<TEMP_Test>();
    
<i>Description:</i>  First is meant to be used to return the first result from a query, DO NOT use with HasNext().
