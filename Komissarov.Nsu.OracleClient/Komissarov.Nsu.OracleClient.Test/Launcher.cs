using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using Komissarov.Nsu.OracleClient.Accessor;
using System.Data.Common;

namespace Komissarov.Nsu.OracleClient.Test
{
	class Launcher
	{
		public static void Main( )
		{
			OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder( );

			builder.DataSource = @"10.4.0.119";
			builder.Password = @"123456";
			builder.UserID = @"12201";
			builder.PersistSecurityInfo = true;

			using ( OracleAccessor connector = new OracleAccessor( "10.4.0.119", "12201", "123456", "USERS" ) )
			{
				using ( var dataReader = connector.GetTableInfo( "ROGOLEV_GOODS" ) )
				{
					//while ( dataReader.Read( ) )
					//{
					//	string str = dataReader.GetString( dataReader.GetOrdinal( "table_name" ) );
					
					//	Console.WriteLine( dataReader[0].ToString( ) );
					//}

					foreach ( DbDataRecord row in dataReader )
					{
						for ( int i = 0; i < row.FieldCount; ++i )
							Console.WriteLine( row.GetValue( i ) );
					}
				}	


			}

			//using ( OracleConnection connection = new OracleConnection( builder.ToString( ) ) )
			//{
			//	connection.Open( );

			//	using ( OracleCommand command = connection.CreateCommand( ) )
			//	{
			//		command.CommandText = @"select * from all_tables";

			//		DataTable table = connection.GetSchema( @"foreignKeyColumns" );

			//		foreach ( DataRow row in table.Rows )
			//		{
			//			if ( !row["TABLE_NAME"].ToString( ).Contains( "ROGOLEV" ) )
			//				continue;
			//			foreach ( DataColumn column in table.Columns )
			//				Console.WriteLine( "{0} = {1}", column.ColumnName, row[column] );
			//			Console.WriteLine( );
			//		}

			//		//using ( var dataReader = command.ExecuteReader( ) )
			//		//{
			//		//	while ( dataReader.Read( ) )
			//		//	{
			//		//		string str = dataReader.GetString( dataReader.GetOrdinal( "table_name" ) );
			//		//		//try
			//		//		//{
			//		//		//	str += " " + dataReader.GetString( dataReader.GetOrdinal( "tablespace_name" ) );
			//		//		//}
			//		//		//catch ( InvalidCastException e )
			//		//		//{
			//		//		//	str += " null";
			//		//		//}
			//		//		Console.WriteLine( dataReader[0].ToString( ) );
			//		//	}
			//		//}
			//	}

			//}


			//	using ( dataReader )
			//	{
			//		while ( dataReader.Read( ) )
			//		{
			//			string str = dataReader.GetString( dataReader.GetOrdinal( "table_name" ) );
			//			//try
			//			//{
			//			//	str += " " + dataReader.GetString( dataReader.GetOrdinal( "tablespace_name" ) );
			//			//}
			//			//catch ( InvalidCastException e )
			//			//{
			//			//	str += " null";
			//			//}
			//			Console.WriteLine( dataReader[0].ToString( ) );
			//		}
			//	}

			//}

			Console.ReadKey( );
		}
	}
}
