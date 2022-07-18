using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace PharmacyApp
{
    /// <summary>
    /// класс для добавления/удаления аптеки в БД
    /// </summary>
    internal class PharmacyManager : ItemManager
    {
        public override string Text() => "Введите 1 для добавления аптеки, 2 для удаления, 3 чтобы отобразить список товаров в аптеке" +
            " и 0 для возврата";        

        public override void CreateItem(SqlConnection connection)
        {
            Console.WriteLine("Введите наименование аптеки: ");
            string name = Console.ReadLine();

            if (name.Trim() == "")
            {
                Console.WriteLine("Наименование не должно быть пустым: ");
                return;
            }

            Console.WriteLine("Введите адрес аптеки: ");
            string address = Console.ReadLine();

            Console.WriteLine("Введите телефон аптеки: ");
            string phone = Console.ReadLine();

            SqlCommand command = new SqlCommand("CreatePharmacy");
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Address", address);
            command.Parameters.AddWithValue("@Phone", phone);
            Execute(command, connection);
        }
        public override void DeleteItem(SqlConnection connection)
        {
            Console.WriteLine("Введите наименование аптеки: ");
            string name = Console.ReadLine();

            SqlCommand command = new SqlCommand("DeletePharmacy", connection);
            command.Parameters.AddWithValue("@Name", name);
            Execute(command, connection);
        }
        /// <summary>
        /// Метод для отображения количество товара во всех складах аптеки
        /// </summary>
        /// <param name="connection">текущее соединение к БД</param>        
        public void ShowAllGoods(SqlConnection connection)
        {
            Console.WriteLine("Введите наименование аптеки: ");
            string name = Console.ReadLine();

            SqlCommand command = new SqlCommand("ShowAllGoods", connection);
            command.Parameters.AddWithValue("@Name", name);
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@RESULT", SqlDbType.NVarChar).Direction = ParameterDirection.Output;
            command.Parameters["@RESULT"].Size = 255;
            try
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    //если колиство строк больше нуля то отображаем
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string goodsName = reader.GetString(0);
                            int count = reader.GetInt32(1);
                            Console.WriteLine("\t{0} \t{1}", goodsName.TrimEnd(), count);
                        }                        
                    }
                    else
                    {
                        //если есть возвращаемая строка то такой аптеки не существует
                        if (command.Parameters["@RESULT"].Value != null) 
                        {
                            Console.WriteLine(command.Parameters["@RESULT"].Value.ToString());
                        }
                        else
                        {
                            Console.WriteLine("На складах выбранной аптеки отсутствует товар");
                        }                                                                       
                    }  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    /// <summary>
    /// класс для добавления/удаления товара в БД
    /// </summary>
    internal class GoodsManager : ItemManager
    {
        public override string Text() => "Введите 1 для добавления товара, 2 для удаления и 0 для возврата";

        public override void CreateItem(SqlConnection connection)
        {
            Console.WriteLine("Введите наименование товара: ");
            string name = Console.ReadLine();

            if (name.Trim() == "")
            {
                Console.WriteLine("Наименование не должно быть пустым: ");
                return;
            }

            SqlCommand command = new SqlCommand("CreateGoods");
            command.Parameters.AddWithValue("@Name", name);
            Execute(command, connection);
        }
        public override void DeleteItem(SqlConnection connection)
        {
            Console.WriteLine("Введите наименование товара: ");
            string name = Console.ReadLine();

            SqlCommand command = new SqlCommand("DeleteGoods", connection);
            command.Parameters.AddWithValue("@Name", name);
            Execute(command, connection);
        }        
    }
    /// <summary>
    /// класс для добавления/удаления склада в БД
    /// </summary>
    internal class WarehouseManager : ItemManager
    {
        public override string Text() => "Введите 1 для добавления склада, 2 для удаления и 0 для возврата";

        public override void CreateItem(SqlConnection connection)
        {
            Console.WriteLine("Введите наименование склада: ");
            string name = Console.ReadLine();

            if (name.Trim() == "")
            {
                Console.WriteLine("Наименование не должно быть пустым: ");
                return;
            }

            Console.WriteLine("Введите наименование аптеки, к которой будет привязан склад: ");
            string pharmacyName = Console.ReadLine();

            SqlCommand command = new SqlCommand("CreateWarehouse");
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@PharmacyName", pharmacyName);
            Execute(command, connection);
        }
        public override void DeleteItem(SqlConnection connection)
        {
            Console.WriteLine("Введите наименование склада: ");
            string name = Console.ReadLine();

            SqlCommand command = new SqlCommand("DeleteWarehouse", connection);
            command.Parameters.AddWithValue("@Name", name);
            Execute(command, connection);
        }
    }
    /// <summary>
    /// класс для добавления/удаления партии в БД
    /// </summary>
    internal class BatchManager : ItemManager
    {
        public override string Text() => "Введите 1 для добавления партии, 2 для удаления и 0 для возврата";

        public override void CreateItem(SqlConnection connection)
        {
            Console.WriteLine("Введите наименование склада, на котором будет заведена партия: ");
            string warehouseName = Console.ReadLine();

            Console.WriteLine("Введите наименование товара этой партии: ");
            string goodsName = Console.ReadLine();

            Console.WriteLine("Введите количество товара этой партии: ");
            if (int.TryParse(Console.ReadLine(), out int count))
            {
                if (count <= 0)
                {
                    Console.WriteLine("Количество товара должно быть положительное ");
                    return;
                }                
            }
            else
            {
                Console.WriteLine("Проверьте правильность введенного значения ");
                return;
            }

            SqlCommand command = new SqlCommand("CreateBatch");
            command.Parameters.AddWithValue("@WarehouseName", warehouseName);
            command.Parameters.AddWithValue("@GoodsName", goodsName);
            command.Parameters.AddWithValue("@Count", count);
            Execute(command, connection);
        }
        public override void DeleteItem(SqlConnection connection)
        {
            Console.WriteLine("Введите ID партии:\n");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (id < 0)
                {
                    Console.WriteLine("ID партии должно быть неотрицательным ");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Проверьте правильность введенного значения ");
                return;
            }

            SqlCommand command = new SqlCommand("DeleteBatch", connection);
            command.Parameters.AddWithValue("@ID", id);
            Execute(command, connection);
        }
    }
    /// <summary>
    /// Абстрактный класс для добавления и удаления элемнтов в базе данных
    /// </summary>    
    internal abstract class ItemManager
    {
        //текст для выбора операции добавления/удаления данных в БД
        public abstract string Text();
        /// <summary>
        /// Метод выполнения хранимой процедуры в БД
        /// </summary>
        /// <param name="command">хранимая процедура для добавления/удаления элемента</param>
        /// <param name="connection">текущее соединение к БД</param>
        protected void Execute(SqlCommand command, SqlConnection connection)
        {
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@RESULT", SqlDbType.NVarChar).Direction = ParameterDirection.Output;
            command.Parameters["@RESULT"].Size = 255;
            try
            {                
                command.ExecuteScalar();
                Console.WriteLine(command.Parameters["@RESULT"].Value.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Метод добавления нового элемента в БД
        /// </summary>
        /// <param name="connection">текущее соединение к БД</param>        
        public abstract void CreateItem(SqlConnection connection);
        /// <summary>
        /// Метод удаления элемента из БД
        /// </summary>
        /// <param name="connection">текущее соединение к БД</param>
        public abstract void DeleteItem(SqlConnection connection);              
    }
}
