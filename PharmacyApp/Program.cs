using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace PharmacyApp
{
    internal class Program
    {
        //строка подключения к БД
        static readonly string connectionString = @"Data Source =.\SQLEXPRESS; Initial Catalog = TestDB; User id=StoredProcedureUser; 
                                                    Password=1234";

        static void Main(string[] args)
        {  
            while (true)
            {
                Console.Write("Введите 1 для просмотра списка товаров, 2 - аптек, 3 - складов, 4 - партий, " +
                    "0 - для выхода из программы");
                Console.WriteLine();
                if (int.TryParse(Console.ReadLine(), out int f))
                {
                    switch (f)
                    {
                        case 0:
                            return;
                        case 1:
                            LoadData("SELECT * FROM Goods");
                            ManageItems(new GoodsManager());
                            break;
                        case 2:
                            LoadData("SELECT * FROM Pharmacies");
                            ManageItems(new PharmacyManager());
                            break;
                        case 3:
                            LoadData("SELECT * FROM Warehouses");
                            ManageItems(new WarehouseManager());
                            break;
                        case 4:
                            LoadData("SELECT * FROM Batches");
                            ManageItems(new BatchManager());
                            break;
                        default:
                            break;
                    }
                } 
            }            
        }
        /// <summary>
        /// Статический метод для работы с элементами из БД
        /// </summary>
        /// <param name="manager">класс для работы с данными БД</param>        
        static void ManageItems(ItemManager manager)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                while (true)
                {
                    Console.WriteLine(manager.Text());
                    if (int.TryParse(Console.ReadLine(), out int f))
                    {
                        switch (f)
                        {
                            case 1:
                                manager.CreateItem(connection);
                                break;
                            case 2:
                                manager.DeleteItem(connection);
                                break;
                            case 0:
                                return;
                            default:
                                break;
                        }
                    }
                }                
            }                                          
        }
        /// <summary>
        /// метод загрузки списка элементов
        /// </summary>
        /// <param name="sql">запрос с выбором всех данных таблицы</param>
        static void LoadData(string sql)
        {            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                DataTable dt = ds.Tables[0];

                //вывод заголовков столбцов
                foreach (DataColumn column in dt.Columns)
                {
                    Console.Write("\t{0}", column.ColumnName);
                }
                Console.WriteLine();

                //вывод данных в строках
                foreach (DataRow row in dt.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        Console.Write("\t{0}", item.ToString().TrimEnd());
                    }
                    Console.WriteLine();  
                }
            }            
        }
    }
}

