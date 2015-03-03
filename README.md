SkyGroundLabs
=============

James custom framework

Entity Framework is currently being reworked and readied for nuget.

#Highlights

1. Currently integrating lambda and custom methods for more natural Sql writing in .NET.  I am in the testing phase right now.

Once done queries will look like this:

    var context = new DbSqlContext("your connection string");
    var query = context.From<MyClass>().Where<MyClass>(w => w.MyProp == "Test").Select<MyClass>();
    // query is a type of ExpressionQuery.  The behavior is just like IQueryable, the enumeration happens when First or All is called.
    var result = query.First();
    
You must pass in the types in the From, Where, and Select methods.  The reason is speed.  Its much faster not having to evaulate the expression each time you want to select a type.  This is where the query builders speed comes from.

Also, joins are more intuitive.  A join looks like this, Join<ParentTable, ChildTable>((p,c) => p.ChildId == c.Id).  A left join is the same code, you just call LeftJoin instead.

##Functions Available:
1. From
2. Where
3. Select
4. Join
5. Left Join
6. Casting
7. Converting


This is temporary documentation, permanent documentation will come once completed.
