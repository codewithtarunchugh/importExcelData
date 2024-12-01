////Program to save all questions links into text file.
//using HtmlAgilityPack;

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        string baseUrl = "https://lawrato.com/high-court-legal-advice?&page=";
//        string outputFilePath = "C:\\My Web Sites\\Links\\high-court-legal-advicelinks.txt";

//        using (var writer = new StreamWriter(outputFilePath))
//        {
//            using (HttpClient client = new HttpClient())
//            {
//                for (int pageNumber = 1; pageNumber <= 3; pageNumber++)
//                {
//                    string url = baseUrl + pageNumber;
//                    string pageContent = await client.GetStringAsync(url);

//                    var doc = new HtmlDocument();
//                    doc.LoadHtml(pageContent);

//                    // XPath to get all question links
//                    var linkNodes = doc.DocumentNode.SelectNodes("//div[@class='title title-smaller']/a");

//                    if (linkNodes != null)
//                    {
//                        // Collect links into a list
//                        var links = new List<string>();
//                        foreach (var link in linkNodes)
//                        {
//                            string href = link.GetAttributeValue("href", string.Empty);
//                            if (!string.IsNullOrEmpty(href))
//                            {
//                                links.Add(href);
//                            }
//                        }

//                        // Write the comma-separated links for this page to the file
//                        await writer.WriteLineAsync(string.Join(",", links));
//                    }

//                    Console.WriteLine($"Processed page {pageNumber}");
//                }
//            }
//        }

//        Console.WriteLine($"Links successfully saved to {outputFilePath}");
//    }
//}

//program to save all data in category wise sheet

//using HtmlAgilityPack;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;

//class Program
//{
//    static void Main(string[] args)
//    {
//        // Set the license context
//        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Change this as needed

//        Console.WriteLine("Starting data extraction...");

//        // Create a new Excel package
//        using var package = new ExcelPackage();

//        // Base directory to start searching
//        string baseDirectory = "C:\\My Web Sites\\lawrato\\lawrato.com";

//        // Get all folders ending with '-advice' and retrieve HTML files
//        var adviceDirectories = Directory.GetDirectories(baseDirectory, "*-advice", SearchOption.AllDirectories);

//        foreach (var directory in adviceDirectories)
//        {
//            // Create a new worksheet for each directory
//            var worksheetName = new DirectoryInfo(directory).Name; // Get the folder name as worksheet name
//            var worksheet = package.Workbook.Worksheets.Add(worksheetName);

//            // Add headers to the Excel sheet
//            worksheet.Cells[1, 1].Value = "S. No";
//            worksheet.Cells[1, 2].Value = "Category";
//            worksheet.Cells[1, 3].Value = "Question Text";
//            worksheet.Cells[1, 4].Value = "User Question Text";
//            worksheet.Cells[1, 5].Value = "Recent Questions"; // New column for recent questions

//            // Format headers
//            using (var headerRange = worksheet.Cells[1, 1, 1, 5])
//            {
//                headerRange.Style.Font.Bold = true;
//                headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//                headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
//                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
//                headerRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
//            }

//            // Initialize the row counter
//            int row = 2;

//            var htmlFiles = Directory.GetFiles(directory, "*.html");

//            foreach (var file in htmlFiles)
//            {
//                var doc = new HtmlDocument();
//                doc.Load(file);

//                // Extract category and question text
//                var spanNodeCategory = doc.DocumentNode.SelectSingleNode("//div[@id='main']//section[@id='breadcrumb']//li[2]/a");
//                var spanNode = doc.DocumentNode.SelectSingleNode("//div[@id='main']//section[@id='breadcrumb']//li[3]/span");

//                string category = spanNodeCategory?.InnerText.Trim() ?? "N/A";
//                string questionText = spanNode?.InnerText.Trim() ?? "N/A";

//                // Add S. No, category, and question text to the Excel sheet
//                worksheet.Cells[row, 1].Value = row - 1; // Serial number
//                worksheet.Cells[row, 2].Value = category;
//                worksheet.Cells[row, 3].Value = questionText;

//                // Extract the actual user question text
//                var questionBodyNode = doc.DocumentNode.SelectSingleNode("//div[@class='question-body']/div[@itemprop='text']");
//                string userQuestionText = questionBodyNode?.InnerText.Trim() ?? "N/A";
//                worksheet.Cells[row, 4].Value = userQuestionText; // Add user question text

//                // Extract recent questions
//                var recentQuestionsNode = doc.DocumentNode.SelectSingleNode("//div[@id='hiderelque']//ul");
//                string recentQuestions = "";

//                if (recentQuestionsNode != null)
//                {
//                    var liNodes = recentQuestionsNode.SelectNodes("./li/a");
//                    if (liNodes != null)
//                    {
//                        foreach (var li in liNodes)
//                        {
//                            recentQuestions += li.InnerText.Trim() + "\n"; // Concatenate with a separator
//                        }
//                    }
//                }

//                worksheet.Cells[row, 5].Value = recentQuestions.TrimEnd('\n'); // Add recent questions to the Excel sheet

//                // Extract answers
//                var answerNodes = doc.DocumentNode.SelectNodes("//div[@class='answer-body']");
//                int answerColumn = 6; // Start writing answers from column F
//                int answerIndex = 1; // To count answers dynamically

//                // Iterate through each answer-body node and extract text
//                if (answerNodes != null)
//                {
//                    foreach (var answerNode in answerNodes)
//                    {
//                        var textNode = answerNode.SelectSingleNode("./div[@itemprop='text']");
//                        if (textNode != null)
//                        {
//                            // Replace <br> with new lines for better readability
//                            var extractedText = textNode.InnerHtml.Replace("<br>", "\n").Trim();

//                            // Add answer text to the Excel sheet in the next column
//                            worksheet.Cells[row, answerColumn].Value = extractedText; // Add to next column

//                            // Set header for answer column dynamically
//                            worksheet.Cells[1, answerColumn].Value = $"Answer {answerIndex++}"; // Answer header

//                            // Set wrap text for the answer cell
//                            worksheet.Cells[row, answerColumn].Style.WrapText = true;

//                            answerColumn++; // Move to the next answer column
//                        }
//                    }
//                }

//                // Set fixed column width for all columns
//                for (int col = 1; col <= answerColumn; col++)
//                {
//                    worksheet.Column(col).Width = 60; // Set width to 60
//                    worksheet.Column(col).Style.WrapText = true;
//                }

//                // Apply alternating row colors
//                if (row % 2 == 0)
//                {
//                    using (var range = worksheet.Cells[row, 1, row, answerColumn - 1])
//                    {
//                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
//                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
//                    }
//                }
//                else
//                {
//                    using (var range = worksheet.Cells[row, 1, row, answerColumn - 1])
//                    {
//                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
//                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
//                    }
//                }

//                // Add borders to the row
//                using (var borderRange = worksheet.Cells[row, 1, row, answerColumn - 1])
//                {
//                    borderRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
//                }

//                // Move to the next row for the next question
//                row++;
//            }
//        }

//        // Save the Excel file
//        var excelFilePath = "C:\\My Web Sites\\lawrato\\lawrato.com\\__law_advice_kanoonkibaat.xlsx";
//        package.SaveAs(new FileInfo(excelFilePath));
//        Console.WriteLine($"Data successfully saved to {excelFilePath}");
//    }
//}

// program to read online 

//using HtmlAgilityPack;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;

//class Program
//{
//    private static readonly HttpClient client = new HttpClient();
//    private static readonly string[] UserAgents = new[]
//    {
//        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36",
//        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Gecko/20100101 Firefox/89.0",
//        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0.1 Safari/605.1.15",
//        // Add more user agents if needed
//    };

//    static async Task Main(string[] args)
//    {
//        // Set the license context
//        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//        Console.WriteLine("Starting data extraction...");

//        // Create a new Excel package
//        using var package = new ExcelPackage();
//        var worksheet = package.Workbook.Worksheets.Add("Law Advice Data");

//        // Add headers to the Excel sheet
//        worksheet.Cells[1, 1].Value = "S. No";
//        worksheet.Cells[1, 2].Value = "Category";
//        worksheet.Cells[1, 3].Value = "Question Text";
//        worksheet.Cells[1, 4].Value = "User Question Text";
//        worksheet.Cells[1, 5].Value = "Recent Questions";

//        // Format headers
//        using (var headerRange = worksheet.Cells[1, 1, 1, 5])
//        {
//            headerRange.Style.Font.Bold = true;
//            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
//            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
//            headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
//            headerRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
//        }

//        // Initialize the row counter
//        int row = 2;

//        // Read links from the text file
//        string[] links = await File.ReadAllLinesAsync(@"C:\\My Web Sites\\lawrato\\familylinks.txt");

//        foreach (var link in links)
//        {
//            // Randomly select a user agent
//            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgents[new Random().Next(UserAgents.Length)]);

//            try
//            {
//                var response = await client.GetStringAsync(link);
//                var doc = new HtmlDocument();
//                doc.LoadHtml(response);

//                // Extract category and question text
//                var spanNodeCategory = doc.DocumentNode.SelectSingleNode("//div[@id='main']//section[@id='breadcrumb']//li[2]/a");
//                var spanNode = doc.DocumentNode.SelectSingleNode("//div[@id='main']//section[@id='breadcrumb']//li[3]/span");

//                string category = spanNodeCategory?.InnerText.Trim() ?? "N/A";
//                string questionText = spanNode?.InnerText.Trim() ?? "N/A";

//                // Add S. No, category, and question text to the Excel sheet
//                worksheet.Cells[row, 1].Value = row - 1; // Serial number
//                worksheet.Cells[row, 2].Value = category;
//                worksheet.Cells[row, 3].Value = questionText;

//                // Extract the actual user question text
//                var questionBodyNode = doc.DocumentNode.SelectSingleNode("//div[@class='question-body']/div[@itemprop='text']");
//                string userQuestionText = questionBodyNode?.InnerText.Trim() ?? "N/A";
//                worksheet.Cells[row, 4].Value = userQuestionText; // Add user question text

//                // Extract recent questions
//                var recentQuestionsNode = doc.DocumentNode.SelectSingleNode("//div[@id='hiderelque']//ul");
//                string recentQuestions = "";

//                if (recentQuestionsNode != null)
//                {
//                    var liNodes = recentQuestionsNode.SelectNodes("./li/a");
//                    if (liNodes != null)
//                    {
//                        foreach (var li in liNodes)
//                        {
//                            recentQuestions += li.InnerText.Trim() + "\n"; // Concatenate with a separator
//                        }
//                    }
//                }

//                worksheet.Cells[row, 5].Value = recentQuestions.TrimEnd('\n'); // Add recent questions to the Excel sheet

//                // Extract answers
//                var answerNodes = doc.DocumentNode.SelectNodes("//div[@class='answer-body']");
//                int answerColumn = 6; // Start writing answers from column F
//                int answerIndex = 1; // To count answers dynamically

//                // Iterate through each answer-body node and extract text
//                if (answerNodes != null)
//                {
//                    foreach (var answerNode in answerNodes)
//                    {
//                        var textNode = answerNode.SelectSingleNode("./div[@itemprop='text']");
//                        if (textNode != null)
//                        {
//                            // Replace <br> with new lines for better readability
//                            var extractedText = textNode.InnerHtml.Replace("<br>", "\n").Trim();

//                            // Add answer text to the Excel sheet in the next column
//                            worksheet.Cells[row, answerColumn].Value = extractedText; // Add to next column

//                            // Set header for answer column dynamically
//                            worksheet.Cells[1, answerColumn].Value = $"Answer {answerIndex++}"; // Answer header

//                            // Set wrap text for the answer cell
//                            worksheet.Cells[row, answerColumn].Style.WrapText = true;

//                            answerColumn++; // Move to the next answer column
//                        }
//                    }
//                }

//                // Set fixed column width for all columns
//                for (int col = 1; col <= answerColumn; col++)
//                {
//                    worksheet.Column(col).Width = 60; // Set width to 60
//                    worksheet.Column(col).Style.WrapText = true;
//                }

//                // Apply alternating row colors
//                if (row % 2 == 0)
//                {
//                    using (var range = worksheet.Cells[row, 1, row, answerColumn - 1])
//                    {
//                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
//                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
//                    }
//                }
//                else
//                {
//                    using (var range = worksheet.Cells[row, 1, row, answerColumn - 1])
//                    {
//                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
//                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
//                    }
//                }

//                // Add borders to the row
//                using (var borderRange = worksheet.Cells[row, 1, row, answerColumn - 1])
//                {
//                    borderRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
//                }

//                // Output URL and S. No to the console
//                Console.WriteLine($"URL: {link}, S. No: {row - 1}");

//                // Delay between requests to avoid being detected
//                await Task.Delay(1000); // Adjust the delay as needed

//                // Move to the next row for the next question
//                row++;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Failed to process {link}: {ex.Message}");
//            }
//        }

//        // Save the Excel file
//        var excelFilePath = "C:\\My Web Sites\\lawrato\\family_law_advice_kanoonkibaat.xlsx";
//        package.SaveAs(new FileInfo(excelFilePath));
//        Console.WriteLine($"Data successfully saved to {excelFilePath}");
//    }
//}


//using HtmlAgilityPack;
//using OfficeOpenXml;


//class Program
//{
//    static void Main(string[] args)
//    {
//        // Set the license context
//        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Change this as needed

//        Console.WriteLine("Starting data extraction...");

//        // Create a new Excel package
//        using var package = new ExcelPackage();

//        // Base directory to start searching
//        string baseDirectory = "C:\\My Web Sites\\lawrato\\lawrato.com";

//        // Get all folders ending with '-advice' and retrieve HTML files
//        var adviceDirectories = Directory.GetDirectories(baseDirectory, "*-advice", SearchOption.AllDirectories);

//        foreach (var directory in adviceDirectories)
//        {
//            // Create a new worksheet for each directory
//            var worksheetName = new DirectoryInfo(directory).Name; // Get the folder name as worksheet name
//            var worksheet = package.Workbook.Worksheets.Add(worksheetName);

//            // Add headers to the Excel sheet
//            worksheet.Cells[1, 1].Value = "S. No";
//            worksheet.Cells[1, 2].Value = "Category";
//            worksheet.Cells[1, 3].Value = "Question Text";
//            worksheet.Cells[1, 4].Value = "User Question Text";
//            worksheet.Cells[1, 5].Value = "Recent Questions"; // New column for recent questions


//            // Initialize the row counter
//            int row = 2;

//            var htmlFiles = Directory.GetFiles(directory, "*.html");

//            foreach (var file in htmlFiles)
//            {
//                var doc = new HtmlDocument();
//                doc.Load(file);

//                // Extract category and question text
//                var spanNodeCategory = doc.DocumentNode.SelectSingleNode("//div[@id='main']//section[@id='breadcrumb']//li[2]/a");
//                var spanNode = doc.DocumentNode.SelectSingleNode("//div[@id='main']//section[@id='breadcrumb']//li[3]/span");

//                string category = spanNodeCategory?.InnerText.Trim() ?? "N/A";
//                string questionText = spanNode?.InnerText.Trim() ?? "N/A";

//                // Add S. No, category, and question text to the Excel sheet
//                worksheet.Cells[row, 1].Value = row - 1; // Serial number
//                worksheet.Cells[row, 2].Value = category;
//                worksheet.Cells[row, 3].Value = questionText;

//                // Extract the actual user question text
//                var questionBodyNode = doc.DocumentNode.SelectSingleNode("//div[@class='question-body']/div[@itemprop='text']");
//                string userQuestionText = questionBodyNode?.InnerText.Trim() ?? "N/A";
//                worksheet.Cells[row, 4].Value = userQuestionText; // Add user question text

//                // Extract recent questions
//                var recentQuestionsNode = doc.DocumentNode.SelectSingleNode("//div[@id='hiderelque']//ul");
//                string recentQuestions = "";

//                if (recentQuestionsNode != null)
//                {
//                    var liNodes = recentQuestionsNode.SelectNodes("./li/a");
//                    if (liNodes != null)
//                    {
//                        foreach (var li in liNodes)
//                        {
//                            recentQuestions += li.InnerText.Trim() + "\n"; // Concatenate with a separator
//                        }
//                    }
//                }

//                worksheet.Cells[row, 5].Value = recentQuestions.TrimEnd('\n'); // Add recent questions to the Excel sheet

//                // Extract answers
//                var answerNodes = doc.DocumentNode.SelectNodes("//div[@class='answer-body']");
//                int answerColumn = 6; // Start writing answers from column F
//                int answerIndex = 1; // To count answers dynamically

//                // Iterate through each answer-body node and extract text
//                if (answerNodes != null)
//                {
//                    foreach (var answerNode in answerNodes)
//                    {
//                        var textNode = answerNode.SelectSingleNode("./div[@itemprop='text']");
//                        if (textNode != null)
//                        {
//                            // Replace <br> with new lines for better readability
//                            var extractedText = textNode.InnerHtml.Replace("<br>", "\n").Trim();

//                            // Add answer text to the Excel sheet in the next column
//                            worksheet.Cells[row, answerColumn].Value = extractedText; // Add to next column

//                            // Set header for answer column dynamically
//                            worksheet.Cells[1, answerColumn].Value = $"Answer {answerIndex++}"; // Answer header

//                            // Set wrap text for the answer cell
//                            worksheet.Cells[row, answerColumn].Style.WrapText = true;

//                            answerColumn++; // Move to the next answer column
//                        }
//                    }
//                }                    



//                // Move to the next row for the next question
//                row++;
//            }
//        }

//        // Save the Excel file
//        var excelFilePath = "C:\\My Web Sites\\lawrato\\lawrato.com\\__law_advice_kanoonkibaat.xlsx";
//        package.SaveAs(new FileInfo(excelFilePath));
//        Console.WriteLine($"Data successfully saved to {excelFilePath}");
//    }
//}

//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Web;
//using HtmlAgilityPack;

//class Program
//{
//    static void Main(string[] args)
//    {
//        // Define the connection string
//        string connectionString = "Data Source=.;Initial Catalog=CRM_AVINO;persist security info=True;User ID=sa;Password=;TrustServerCertificate=True";

//        // Base directory to start searching
//        string baseDirectory = "F:\\lawrato\\lawrato.com\\indian-kanoon";

//        try
//        {
//            // Create a new SqlConnection object
//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                // Open the connection
//                connection.Open();

//                // Get all folders ending with '-advice' and retrieve HTML files
//                var adviceDirectories = System.IO.Directory.GetDirectories(baseDirectory, "*-law", System.IO.SearchOption.AllDirectories);

//                foreach (var directory in adviceDirectories)
//                {
//                    var htmlFiles = System.IO.Directory.GetFiles(directory, "*.html");

//                    foreach (var file in htmlFiles)
//                    {
//                        var doc = new HtmlDocument();
//                        doc.Load(file);

//                        // Extract category and question text
//                        var spanNodeCategory = doc.DocumentNode.SelectSingleNode("//div[@id='main']//section[@id='breadcrumb']//li[2]/a");
//                        var spanNode = doc.DocumentNode.SelectSingleNode("//div[@id='main']//section[@id='breadcrumb']//li[3]/span");

//                        string categoryName = spanNodeCategory?.InnerText.Trim().Replace("Law Guide","") ?? "N/A";
//                        string pageName = spanNode?.InnerText.Trim().Replace(":","").Replace(" ","-") ?? "N/A";
//                        string questionText = spanNode?.InnerText.Trim() ?? "N/A";

//                        // Check if the category already exists
//                        string categoryQuery = "SELECT Id FROM Categories WHERE CategoryName = @CategoryName";
//                        using (SqlCommand command = new SqlCommand(categoryQuery, connection))
//                        {
//                            command.Parameters.AddWithValue("@CategoryName", categoryName);
//                            Console.WriteLine($"Executing query: {categoryQuery} with values: CategoryName = {categoryName}");
//                            object categoryIdObject = command.ExecuteScalar();

//                            int categoryId = 0;
//                            if (categoryIdObject != DBNull.Value)
//                            {
//                                categoryId = Convert.ToInt32(categoryIdObject);
//                            }

//                            if (categoryId == 0)
//                            {
//                                // Insert the category if it doesn't exist
//                                string insertCategoryQuery = "INSERT INTO Categories (CategoryName) OUTPUT INSERTED.Id VALUES (@CategoryName)";
//                                using (SqlCommand insertCommand = new SqlCommand(insertCategoryQuery, connection))
//                                {
//                                    insertCommand.Parameters.AddWithValue("@CategoryName", categoryName);
//                                    Console.WriteLine($"Executing query: {insertCategoryQuery} with values: CategoryName = {categoryName}");
//                                    categoryIdObject = insertCommand.ExecuteScalar();

//                                    if (categoryIdObject != DBNull.Value)
//                                    {
//                                        categoryId = Convert.ToInt32(categoryIdObject);
//                                    }
//                                }
//                            }

//                            // Insert the question
//                            string insertQuestionQuery = "INSERT INTO Questions (CategoryId, QuestionText, UserQuestionText) OUTPUT INSERTED.Id VALUES (@CategoryId, @QuestionText, @UserQuestionText)";
//                            using (SqlCommand insertQuestionCommand = new SqlCommand(insertQuestionQuery, connection))
//                            {
//                                insertQuestionCommand.Parameters.AddWithValue("@CategoryId", categoryId);
//                                insertQuestionCommand.Parameters.AddWithValue("@QuestionText", HttpUtility.HtmlEncode(questionText));
//                                insertQuestionCommand.Parameters.AddWithValue("@UserQuestionText", HttpUtility.HtmlEncode(GetUserQuestionText(doc)));
//                                Console.WriteLine($" Executing query: {insertQuestionQuery} with values: CategoryId = {categoryId}, QuestionText = {questionText}, UserQuestionText = {GetUserQuestionText(doc)}");
//                                object questionIdObject = insertQuestionCommand.ExecuteScalar();

//                                int questionId = 0;
//                                if (questionIdObject != DBNull.Value)
//                                {
//                                    questionId = Convert.ToInt32(questionIdObject);
//                                }

//                                // Extract answers
//                                var answerNodes = doc.DocumentNode.SelectNodes("//div[@class='answer-body']");

//                                if (answerNodes != null)
//                                {
//                                    int answerOrder = 1;
//                                    foreach (var answerNode in answerNodes)
//                                    {
//                                        var textNode = answerNode.SelectSingleNode("./div[@itemprop='text']");
//                                        if (textNode != null)
//                                        {
//                                            string answerText = textNode.InnerHtml.Replace("<br>", "\n").Trim();

//                                            // Insert the answer
//                                            string insertAnswerQuery = "INSERT INTO Answers (QuestionId, AnswerText, AnswerOrder) VALUES (@QuestionId, @AnswerText, @AnswerOrder)";
//                                            using (SqlCommand insertAnswerCommand = new SqlCommand(insertAnswerQuery, connection))
//                                            {
//                                                insertAnswerCommand.Parameters.AddWithValue("@QuestionId", questionId);
//                                                insertAnswerCommand.Parameters.AddWithValue("@AnswerText", HttpUtility.HtmlEncode(answerText));
//                                                insertAnswerCommand.Parameters.AddWithValue("@AnswerOrder", answerOrder);
//                                                Console.WriteLine($"Executing query: {insertAnswerQuery} with values: QuestionId = {questionId}, AnswerText = {answerText}, AnswerOrder = {answerOrder}");
//                                                insertAnswerCommand.ExecuteNonQuery();
//                                            }

//                                            answerOrder++;
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine("An error occurred: " + ex.Message);
//        }
//    }

//    static string GetUserQuestionText(HtmlDocument doc)
//    {
//        var questionBodyNode = doc.DocumentNode.SelectSingleNode("//div[@class='question-body']/div[@itemprop='text']");
//        // string userQuestionText =  "N/A";

//        if (questionBodyNode != null)
//        {
//            return questionBodyNode.InnerText.Trim();
//        }
//        else
//        {
//            return "N/A";
//        }
//    }
//}


/*
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using HtmlAgilityPack;

class Program
{
    static void Main(string[] args)
    {
        // Define the connection string
        //string connectionString = "Data Source=.;Initial Catalog=CRM_AVINO;persist security info=True;User ID=sa;Password=;TrustServerCertificate=True";
        string connectionString = "Data Source=sql8002.site4now.net;Initial Catalog=db_a94bc9_crm;persist security info=True;User ID=db_a94bc9_crm_admin;Password=AFvpc8943a;TrustServerCertificate=True";

        // Base directory to start searching
        string baseDirectory = "F:\\lawrato\\lawrato.com\\legal-documents\\";

        try
        {
            // Create a new SqlConnection object
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Get all folders ending with '-advice' and retrieve HTML files
                var adviceDirectories = System.IO.Directory.GetDirectories(baseDirectory, "will-legal-forms", System.IO.SearchOption.AllDirectories);
                //var adviceDirectories = System.IO.Directory.GetDirectories(baseDirectory, "*-law", System.IO.SearchOption.AllDirectories);

                foreach (var directory in adviceDirectories)
                {
                    var htmlFiles = System.IO.Directory.GetFiles(directory, "*.html");

                    foreach (var file in htmlFiles)

                    {
                        var doc = new HtmlDocument();
                        doc.Load(file);

                        // Extract category and question text
                        var spanNodeCategory = doc.DocumentNode.SelectSingleNode("//div[@id='main']//section[@id='breadcrumb']//li[2]/a");
                        var spanNode = doc.DocumentNode.SelectSingleNode("//div[@id='main']//section[@id='breadcrumb']//li[3]/span");

                        //string categoryName = spanNodeCategory?.InnerText.Trim().Replace("Law Guide", "") ?? "N/A";
                        string categoryName = "Wills and Trusts";
                        string pageName = Path.GetFileNameWithoutExtension(file);
                        //string pageName = spanNode?.InnerText.Trim().Replace(":", "").Replace(" ", "-") ?? "N/A";


                        // Check if the category already exists
                        string categoryQuery = "SELECT Id FROM Categories WHERE CategoryName = @CategoryName";
                        using (SqlCommand command = new SqlCommand(categoryQuery, connection))
                        {
                            command.Parameters.AddWithValue("@CategoryName", categoryName);
                            Console.WriteLine($"Executing query: {categoryQuery} with values: CategoryName = {categoryName}");
                            object categoryIdObject = command.ExecuteScalar();

                            int categoryId = 0;
                            if (categoryIdObject != DBNull.Value)
                            {
                                categoryId = Convert.ToInt32(categoryIdObject);
                            }

                            if (categoryId == 0)
                            {
                                // Insert the category if it doesn't exist
                                string insertCategoryQuery = "INSERT INTO Categories (CategoryName) OUTPUT INSERTED.Id VALUES (@CategoryName)";
                                using (SqlCommand insertCommand = new SqlCommand(insertCategoryQuery, connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@CategoryName", categoryName);
                                    Console.WriteLine($"Executing query: {insertCategoryQuery} with values: CategoryName = {categoryName}");
                                    categoryIdObject = insertCommand.ExecuteScalar();

                                    if (categoryIdObject != DBNull.Value)
                                    {
                                        categoryId = Convert.ToInt32(categoryIdObject);
                                    }
                                }
                            }
                            var contentTypeId = 6;
                            string h1Text = "";
                            // Extract answers
                            var answerNodes12 = doc.DocumentNode.SelectNodes("//div[@class='col-sm-12']");
                            var feaheredText = doc.DocumentNode.SelectSingleNode("//h1");
                            if (feaheredText != null)
                            {
                                h1Text = feaheredText.InnerText.Trim();  // Extract and trim the text from the h1 tag
                                Console.WriteLine(h1Text);  // Print or use the text as needed
                            }
                            else
                            {
                                Console.WriteLine("No h1 tag found.");
                            }


                            var answerNodes = doc.DocumentNode.SelectNodes("//div[@class='col-sm-8']");

                            if (answerNodes != null)
                            {
                                // Select the first col-sm-12 node
                                var colSm12 = doc.DocumentNode.SelectSingleNode("//div[@class='col-sm-12']");
                                if (colSm12 != null)
                                {
                                    var text12 = colSm12.OuterHtml;  // Get the outer HTML of col-sm-12

                                    // Select the first col-sm-8 node
                                    var colSm8 = doc.DocumentNode.SelectSingleNode("//div[@class='col-sm-8']");
                                    if (colSm8 != null)
                                    {
                                        // Find the last block-item node inside col-sm-8
                                        var lastNode = colSm8.SelectSingleNode(".//div[@class='block-item'][last()]");

                                        if (lastNode != null)
                                        {
                                            // Get the HTML before the last block-item
                                            var colSm8Html = colSm8.OuterHtml;

                                            // Safely truncate colSm8 HTML before the lastNode.InnerHtml
                                            var lastNodeIndex = colSm8Html.IndexOf(lastNode.OuterHtml);
                                            if (lastNodeIndex > 0)
                                            {
                                             //   colSm8Html = colSm8Html.Substring(0, lastNodeIndex);
                                            }

                                            var text = text12 + colSm8Html;  // Combine col-sm-12 and truncated col-sm-8
                                            text = text.Replace("div class=\"col-sm-12\"", "div").Replace("div class=\"col-sm-8\"", "div").Replace("<p>", "<p style=\"text-align:justify\">").Replace("class=\"content-left\"", "class=\"content-left\" style=\"padding-right:0px!important\"");

                                            // Prepare and execute the insert query
                                            string insertAnswerQuery = "INSERT INTO content (contentTypeId, categoryId, content, pageName, featuredImage,featuredText) VALUES (@contentTypeId, @categoryId, @content, @pageName, @featuredImage,@featuredText)";
                                            using (SqlCommand insertAnswerCommand = new SqlCommand(insertAnswerQuery, connection))
                                            {
                                                insertAnswerCommand.Parameters.AddWithValue("@contentTypeId", contentTypeId);
                                                insertAnswerCommand.Parameters.AddWithValue("@content", text);  // Use the combined text
                                                insertAnswerCommand.Parameters.AddWithValue("@categoryId", categoryId);
                                                insertAnswerCommand.Parameters.AddWithValue("@pageName", pageName.Replace("(","").Replace(")", ""));
                                                insertAnswerCommand.Parameters.AddWithValue("@featuredImage", "assets/images/lawguide-small.png");
                                                insertAnswerCommand.Parameters.AddWithValue("@featuredText", h1Text.Replace("\n",""));
                                                

                                                insertAnswerCommand.ExecuteNonQuery();
                                            }
                                        }
                                       // else
                                      //  {
                                            // Handle the case where no block-item is found
                                      //      Console.WriteLine("No block-item found in col-sm-8.");
                                       // }
                                    }
                                    else
                                    {
                                        // Handle the case where no col-sm-8 node is found
                                        Console.WriteLine("No col-sm-8 found.");
                                    }
                                }
                                else
                                {
                                    // Handle the case where no col-sm-12 node is found
                                    Console.WriteLine("No col-sm-12 found.");
                                }
                            }

                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}*/
//List<string> keywords = new List<string> { "top-stories", "supreme-court", "high-court", "news-updates", "articles", "law-firms", "tax-cases", "consumer-cases", "book-reviews" };
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ClosedXML.Excel;

class Program
{
    static async Task Main(string[] args)
    {
        string url = "https://www.livelaw.in/";

        // List of keywords to filter URLs
        List<string> keywords = new List<string> { "top-stories", "supreme-court", "high-court", "news-updates", "articles", "law-firms", "tax-cases", "consumer-cases", "book-reviews" };

        // Get the HTML content of the page
        var htmlContent = await GetHtmlAsync(url);

        if (!string.IsNullOrEmpty(htmlContent))
        {
            // Extract hrefs from all a tags
            List<string> hrefs = ExtractHrefs(htmlContent);

            // Filter the hrefs based on the keywords with pattern /keyword/ and ending with -numbers
            List<(string, string)> filteredHrefs = FilterUrlsByPatternAndNumber(hrefs, keywords);

            // Visit each filtered URL and extract the <h1> text, date, judgment URL, and tags
            var urlWithDetails = await GetDetailsFromUrls(filteredHrefs);

            // Save the filtered URLs, headings, dates, judgment URLs, and tags to an Excel file
            SaveUrlsToExcel(urlWithDetails, "FilteredUrlsWithHeadingsAndTags.xlsx");
        }
    }

    // Function to fetch the HTML content from a URL
    static async Task<string> GetHtmlAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string htmlContent = await response.Content.ReadAsStringAsync();
                return htmlContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching HTML content: {ex.Message}");
                return string.Empty;
            }
        }
    }

    // Function to extract all hrefs from a tags
    static List<string> ExtractHrefs(string htmlContent)
    {
        List<string> hrefList = new List<string>();
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        var aTags = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
        if (aTags != null)
        {
            foreach (var aTag in aTags)
            {
                string href = aTag.GetAttributeValue("href", string.Empty);
                if (!string.IsNullOrEmpty(href))
                {
                    hrefList.Add(href);
                }
            }
        }

        return hrefList;
    }

    // Function to filter URLs based on the pattern /keyword/ and ending with -numbers
    static List<(string, string)> FilterUrlsByPatternAndNumber(List<string> hrefs, List<string> keywords)
    {
        List<(string, string)> filteredHrefs = new List<(string, string)>();

        foreach (var href in hrefs)
        {
            foreach (var keyword in keywords)
            {
                // Check if the URL contains the keyword in /keyword/ pattern and ends with -numbers
                if (Regex.IsMatch(href, $@"/{keyword}/.*-\d+$", RegexOptions.IgnoreCase))
                {
                    filteredHrefs.Add((href, keyword)); // Add href and keyword as a tuple
                }
            }
        }

        return filteredHrefs;
    }

    // Function to visit each URL, extract <h1> text, date, judgment URL, and tags
    static async Task<List<(string url, string category, string heading, string date, string judgmentUrl, string tags)>> GetDetailsFromUrls(List<(string url, string category)> filteredHrefs)
    {
        List<(string url, string category, string heading, string date, string judgmentUrl, string tags)> result = new List<(string url, string category, string heading, string date, string judgmentUrl, string tags)>();

        foreach (var (url, category) in filteredHrefs)
        {
            string fullUrl = AddUrlPrefix(url);  // Ensure proper URL format
            Console.WriteLine(fullUrl);
            string heading = await GetH1TextFromUrl(fullUrl);
            string date = await GetDateFromUrl(fullUrl);  // Extract date
            string judgmentUrl = await GetJudgmentUrl(fullUrl); // Extract judgment URL
            string tags = await GetTagsFromUrl(fullUrl);  // Extract tags
            result.Add((fullUrl, category, heading, date, judgmentUrl, tags));
        }

        return result;
    }

    // Function to add the correct URL prefix if necessary
    static string AddUrlPrefix(string url)
    {
        if (!url.Contains("https://www.livelaw.in/") && !url.Contains("hindi.livelaw.in/"))
        {
            return $"https://www.livelaw.in/{url.TrimStart('/')}";
        }

        return url;
    }

    // Function to extract the <h1> text from a URL
    static async Task<string> GetH1TextFromUrl(string url)
    {
        string htmlContent = await GetHtmlAsync(url);
        if (!string.IsNullOrEmpty(htmlContent))
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var h1Tag = htmlDoc.DocumentNode.SelectSingleNode("//h1");
            if (h1Tag != null)
            {
                return h1Tag.InnerText.Trim();
            }
        }

        return "No heading found"; // Return this if no <h1> tag is found
    }

    // Function to extract the date from a URL (return only the date, no time)
    static async Task<string> GetDateFromUrl(string url)
    {
        string htmlContent = await GetHtmlAsync(url);
        if (!string.IsNullOrEmpty(htmlContent))
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var dateTag = htmlDoc.DocumentNode.SelectSingleNode("//p[@data-datestring]");
            if (dateTag != null)
            {
                string fullDate = dateTag.GetAttributeValue("data-datestring", "No date found");
                return DateTime.TryParse(fullDate, out DateTime parsedDate) ? parsedDate.ToString("yyyy-MM-dd") : "No date found";
            }
        }

        return "No date found"; // Return this if no <p data-datestring> tag is found
    }

    // Function to extract the judgment URL from the page
    static async Task<string> GetJudgmentUrl(string url)
    {
        string htmlContent = await GetHtmlAsync(url);
        if (!string.IsNullOrEmpty(htmlContent))
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var judgmentLink = htmlDoc.DocumentNode.SelectSingleNode("//a[contains(text(), 'Click Here To Read/Download Judgment')]");
            if (judgmentLink != null)
            {
                return judgmentLink.GetAttributeValue("href", "No judgment URL found");
            }
        }

        return "No judgment URL found"; // Return this if no judgment link is found
    }

    // Function to extract the tags from the page
    static async Task<string> GetTagsFromUrl(string url)
    {
        string htmlContent = await GetHtmlAsync(url);
        if (!string.IsNullOrEmpty(htmlContent))
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var tagsDiv = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'news_details_tags_details')]");
            if (tagsDiv != null)
            {
                var aTags = tagsDiv.SelectNodes(".//a");
                if (aTags != null)
                {
                    var tagTexts = aTags.Select(aTag => aTag.InnerText.Trim());
                    return string.Join(", ", tagTexts); // Join tag texts with comma separator
                }
            }
        }

        return "No tags found"; // Return this if no tags are found
    }

    // Function to save filtered URLs, headings, dates, judgment URLs, tags, and apply formatting
    static void SaveUrlsToExcel(List<(string url, string category, string heading, string date, string judgmentUrl, string tags)> urlWithDetails, string filePath)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Filtered URLs");

            // Add headers with capitalized first letters
            worksheet.Cell(1, 1).Value = "S.No.";
            worksheet.Cell(1, 2).Value = "Date";
            worksheet.Cell(1, 3).Value = "Category";
            worksheet.Cell(1, 4).Value = "Headline";
            worksheet.Cell(1, 5).Value = "Tags";
            worksheet.Cell(1, 6).Value = "Judgment URL";
            worksheet.Cell(1, 7).Value = "URL";

            // Set header row background color
            var headerRange = worksheet.Range("A1:G1");
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Font.Bold = true;

            int row = 2;
            foreach (var (url, category, heading, date, judgmentUrl, tags) in urlWithDetails)
            {
                worksheet.Cell(row, 1).Value = row - 1; // Serial number
                worksheet.Cell(row, 2).Value = date;
                worksheet.Cell(row, 3).Value = category;
                worksheet.Cell(row, 4).Value = heading;
                worksheet.Cell(row, 5).Value = tags;
                worksheet.Cell(row, 6).Value = judgmentUrl;
                worksheet.Cell(row, 7).Value = url;
                row++;
            }

            // Add borders and alternate row colors
            var tableRange = worksheet.Range($"A1:G{row - 1}");
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Apply alternate row colors
            for (int i = 2; i < row; i++)
            {
                if (i % 2 == 0)
                {
                    worksheet.Row(i).Style.Fill.BackgroundColor = XLColor.LightBlue;
                }
                else
                {
                    worksheet.Row(i).Style.Fill.BackgroundColor = XLColor.White;
                }
            }
            // Autofit columns for better visibility
            worksheet.Columns().AdjustToContents();

            // Save the file
            workbook.SaveAs(filePath);
        }
    }
}
*/
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Threading.Tasks;
//using HtmlAgilityPack;
//using ClosedXML.Excel;

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        string baseUrl = "https://www.latestlaws.com/bare-acts/state-acts-rules?page=";
//        int totalPages = 256; // Total number of pages to process
//        int serialNumber = 1; // Start serial number from 1

//        var records = new List<(int SNo, string Rule, string StateActList, string StateName, string FileUrl)>();

//        for (int page = 1; page <= totalPages; page++)
//        {
//            string pageUrl = $"{baseUrl}{page}";
//            Console.WriteLine(pageUrl);
//            await Task.Delay(1000);
//            string htmlContent = await GetHtmlAsync(pageUrl);

//            if (!string.IsNullOrEmpty(htmlContent))
//            {
//                var pageRecords = await ExtractDataFromPage(htmlContent, serialNumber);
//                records.AddRange(pageRecords);
//                serialNumber += pageRecords.Count;
//            }
//        }

//        // Save records to Excel
//        SaveToExcel(records, "StateActsAndRules.xlsx");
//    }

//    // Function to fetch HTML content
//    static async Task<string> GetHtmlAsync(string url)
//    {
//        using (HttpClient client = new HttpClient())
//        {
//            try
//            {
//                HttpResponseMessage response = await client.GetAsync(url);
//                response.EnsureSuccessStatusCode();
//                return await response.Content.ReadAsStringAsync();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error fetching HTML from {url}: {ex.Message}");
//                return string.Empty;
//            }
//        }
//    }

//    // Function to extract data from a page
//    static async Task<List<(int SNo, string Rule, string StateActList, string StateName, string FileUrl)>> ExtractDataFromPage(string htmlContent, int startSerialNumber)
//    {
//        List<(int, string, string, string, string)> records = new List<(int, string, string, string, string)>();

//        HtmlDocument htmlDoc = new HtmlDocument();
//        htmlDoc.LoadHtml(htmlContent);

//        // Locate the table
//        var tableRows = htmlDoc.DocumentNode.SelectNodes("//table[@class='table table-striped table-bordered table-p5']//tr");

//        if (tableRows != null)
//        {
//            int serialNumber = startSerialNumber;

//            foreach (var row in tableRows.Skip(1)) // Skip the header row
//            {
//                var cells = row.SelectNodes("td");
//                if (cells != null && cells.Count >= 3)
//                {
//                    // Extract the required values
//                    string rule = "state-act"; // Default rule value
//                    string stateActList = cells[0].SelectSingleNode(".//a")?.InnerText.Trim() ?? "No Title";
//                    string stateName = cells[2].InnerText.Trim();
//                    string detailPageUrl = cells[0].SelectSingleNode(".//a")?.GetAttributeValue("href", string.Empty);

//                    // Visit the detail page to get the file URL
//                    string fileUrl = await GetFileUrlFromDetailPage(detailPageUrl);

//                    // Add the record
//                    records.Add((serialNumber++, rule, stateActList, stateName, fileUrl));
//                }
//            }
//        }

//        return records;
//    }

//    // Function to visit the detail page and get the file URL from the iframe
//    static async Task<string> GetFileUrlFromDetailPage(string detailPageUrl)
//    {
//        if (string.IsNullOrEmpty(detailPageUrl))
//            return "No File URL";
//        await Task.Delay(1000);
//        string fullUrl = AddUrlPrefix(detailPageUrl);
//        Console.WriteLine(fullUrl);
//        string htmlContent = await GetHtmlAsync(fullUrl);

//        if (!string.IsNullOrEmpty(htmlContent))
//        {
//            HtmlDocument htmlDoc = new HtmlDocument();
//            htmlDoc.LoadHtml(htmlContent);

//            var iframeNode = htmlDoc.DocumentNode.SelectSingleNode("//iframe[@src]");
//            if (iframeNode != null)
//            {
//                string iframeUrl = iframeNode.GetAttributeValue("src", string.Empty);

//                // Replace "preview" with "edit" in the URL
//                return iframeUrl.Replace("preview", "edit");
//            }
//        }

//        return "No File URL";
//    }

//    // Function to ensure full URL prefix
//    static string AddUrlPrefix(string url)
//    {
//        if (!url.StartsWith("https://"))
//        {
//            return $"https://www.latestlaws.com{url}";
//        }

//        return url;
//    }

//    // Function to save records to Excel
//    static void SaveToExcel(List<(int SNo, string Rule, string StateActList, string StateName, string FileUrl)> records, string filePath)
//    {
//        using (var workbook = new XLWorkbook())
//        {
//            var worksheet = workbook.Worksheets.Add("State Acts & Rules");

//            // Add headers
//            worksheet.Cell(1, 1).Value = "S.No";
//            worksheet.Cell(1, 2).Value = "Rule";
//            worksheet.Cell(1, 3).Value = "State Acts & Rules List";
//            worksheet.Cell(1, 4).Value = "State Name";
//            worksheet.Cell(1, 5).Value = "File URL";

//            // Style headers
//            var headerRange = worksheet.Range("A1:E1");
//            headerRange.Style.Font.Bold = true;
//            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

//            // Add data
//            for (int i = 0; i < records.Count; i++)
//            {
//                var record = records[i];

//                worksheet.Cell(i + 2, 1).Value = record.SNo;
//                worksheet.Cell(i + 2, 2).Value = record.Rule;
//                worksheet.Cell(i + 2, 3).Value = record.StateActList;
//                worksheet.Cell(i + 2, 4).Value = record.StateName;
//                worksheet.Cell(i + 2, 5).Value = record.FileUrl;

//                // Apply borders
//                for (int j = 1; j <= 5; j++)
//                {
//                    worksheet.Cell(i + 2, j).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
//                }
//            }

//            // Auto-adjust column widths
//            worksheet.Columns().AdjustToContents();

//            // Save the Excel file
//            workbook.SaveAs(filePath);
//        }
//    }
//}



//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using HtmlAgilityPack;
//using ClosedXML.Excel;

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        string url = "https://latestlaws.com/";

//        // List of keywords to filter URLs
//        List<string> keywords = new List<string> { "case-analysis", "latest-news", "articles", "human-rights-news", "law-firm-news", "international-news"};

//        // Get the HTML content of the page
//        var htmlContent = await GetHtmlAsync(url);

//        if (!string.IsNullOrEmpty(htmlContent))
//        {
//            // Extract hrefs from all a tags
//            List<string> hrefs = ExtractHrefs(htmlContent);

//            // Filter the hrefs based on the keywords with pattern /keyword/ and ending with -numbers
//            List<(string, string)> filteredHrefs = FilterUrlsByPatternAndNumber(hrefs, keywords);

//            // Visit each filtered URL and extract the <h1> text, date, judgment URL, and tags
//            var urlWithDetails = await GetDetailsFromUrls(filteredHrefs);

//            // Save the filtered URLs, headings, dates, judgment URLs, and tags to an Excel file
//            SaveUrlsToExcel(urlWithDetails, "FilteredUrlsWithHeadingsAndTags.xlsx");
//        }
//    }

//    // Function to fetch the HTML content from a URL
//    static async Task<string> GetHtmlAsync(string url)
//    {
//        using (HttpClient client = new HttpClient())
//        {
//            try
//            {
//                HttpResponseMessage response = await client.GetAsync(url);
//                response.EnsureSuccessStatusCode();
//                string htmlContent = await response.Content.ReadAsStringAsync();
//                return htmlContent;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error fetching HTML content: {ex.Message}");
//                return string.Empty;
//            }
//        }
//    }

//    // Function to extract all hrefs from a tags
//    static List<string> ExtractHrefs(string htmlContent)
//    {
//        List<string> hrefList = new List<string>();
//        HtmlDocument htmlDoc = new HtmlDocument();
//        htmlDoc.LoadHtml(htmlContent);

//        var aTags = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
//        if (aTags != null)
//        {
//            foreach (var aTag in aTags)
//            {
//                string href = aTag.GetAttributeValue("href", string.Empty);
//                if (!string.IsNullOrEmpty(href))
//                {
//                    hrefList.Add(href);
//                }
//            }
//        }

//        return hrefList;
//    }

//    // Function to filter URLs based on the pattern /keyword/ and ending with -numbers
//    static List<(string, string)> FilterUrlsByPatternAndNumber(List<string> hrefs, List<string> keywords)
//    {
//        // This will store the unique hrefs we want to return.
//        List<(string, string)> filteredHrefs = new List<(string, string)>();

//        // HashSet to track unique URLs, ignoring the keyword part.
//        HashSet<string> seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // Case-insensitive check

//        foreach (var href in hrefs)
//        {
//            string normalizedHref = href.Trim();  // Trim any surrounding spaces
//            if (string.IsNullOrWhiteSpace(normalizedHref)) continue;

//            foreach (var keyword in keywords)
//            {
//                // Check if the URL matches the pattern, case-insensitive
//                if (Regex.IsMatch(normalizedHref, $@"https?://[^/]+/{keyword}(/.*)?$", RegexOptions.IgnoreCase))
//                {
//                    // If we haven't already seen this URL, add it
//                    if (seenUrls.Add(normalizedHref))
//                    {
//                        filteredHrefs.Add((normalizedHref, keyword)); // Add the URL and keyword tuple
//                    }
//                }
//            }
//        }

//        return filteredHrefs;
//    }
//    // Function to visit each URL, extract <h1> text, date, judgment URL, and tags
//    static async Task<List<(string url, string category, string heading, string date, string judgmentUrl, string tags)>> GetDetailsFromUrls(List<(string url, string category)> filteredHrefs)
//    {
//        List<(string url, string category, string heading, string date, string judgmentUrl, string tags)> result = new List<(string url, string category, string heading, string date, string judgmentUrl, string tags)>();

//        foreach (var (url, category) in filteredHrefs)
//        {
//            string fullUrl = AddUrlPrefix(url);  // Ensure proper URL format
//            Console.WriteLine(fullUrl);
//            string htmlContent = await GetHtmlAsync(fullUrl);
//            await Task.Delay(500);
//            string heading = await GetH1TextFromUrl(fullUrl, htmlContent);
//            string date = await GetDateFromUrl(fullUrl, htmlContent);  // Extract date
//            string judgmentUrl = await GetJudgmentUrl(fullUrl, htmlContent); // Extract judgment URL
//            string tags = await GetTagsFromUrl(fullUrl, htmlContent);  // Extract tags
//            result.Add((fullUrl, category, heading, date, judgmentUrl, tags));
//        }

//        return result;
//    }

//    // Function to add the correct URL prefix if necessary
//    static string AddUrlPrefix(string url)
//    {
//        //if (!url.Contains("https://www.latestlaws.com/") && !url.Contains("hindi.latestlaws.cpm/"))
//        //{
//        //    return $"https://www.latestlaws.com/{url.TrimStart('/')}";
//        //}

//        return url;
//    }

//    // Function to extract the <h1> text from a URL
//    static async Task<string> GetH1TextFromUrl(string url,string _htmlContent)
//    {
//        string htmlContent = _htmlContent;
//        if (!string.IsNullOrEmpty(htmlContent))
//        {
//            HtmlDocument htmlDoc = new HtmlDocument();
//            htmlDoc.LoadHtml(htmlContent);

//            var h1Tag = htmlDoc.DocumentNode.SelectSingleNode("//h1");
//            if (h1Tag != null)
//            {
//                return h1Tag.InnerText.Trim();
//            }
//        }

//        return "No heading found"; // Return this if no <h1> tag is found
//    }

//    // Function to extract the date from a URL (return only the date, no time)
//    static async Task<string> GetDateFromUrl(string url, string _htmlContent)
//    {
//        string htmlContent = _htmlContent;
//        if (!string.IsNullOrEmpty(htmlContent))
//        {
//            HtmlDocument htmlDoc = new HtmlDocument();
//            htmlDoc.LoadHtml(htmlContent);

//            var divTag = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'col-md-6 col-4 text-right line-height-2')]");

//            if (divTag != null)
//            {
//                string divValue = divTag.InnerText.Trim(); // Get the inner text of the div
//                return divValue; // Return the value or process it further as needed
//            }
//            else
//            {
//                return "No div found";
//            }
//        }

//        return "No date found"; // Return this if no <p data-datestring> tag is found
//    }

//    // Function to extract the judgment URL from the page
//    static async Task<string> GetJudgmentUrl(string url, string _htmlContent)
//    {
//        string htmlContent = _htmlContent;
//        if (!string.IsNullOrEmpty(htmlContent))
//        {
//            HtmlDocument htmlDoc = new HtmlDocument();
//            htmlDoc.LoadHtml(htmlContent);

//            var judgmentLink = htmlDoc.DocumentNode.SelectSingleNode("//a[contains(text(), 'Click Here To Read/Download Judgment')]");
//            if (judgmentLink != null)
//            {
//                return judgmentLink.GetAttributeValue("href", "No judgment URL found");
//            }
//        }

//        return "No judgment URL found"; // Return this if no judgment link is found
//    }

//    // Function to extract the tags from the page
//    static async Task<string> GetTagsFromUrl(string url, string _htmlContent)
//    {
//        string htmlContent = _htmlContent;
//        if (!string.IsNullOrEmpty(htmlContent))
//        {
//            HtmlDocument htmlDoc = new HtmlDocument();
//            htmlDoc.LoadHtml(htmlContent);

//            var tagsDiv = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'linked-post-tags')]");
//            if (tagsDiv != null)
//            {
//                var aTags = tagsDiv.SelectNodes(".//a");
//                if (aTags != null)
//                {
//                    var tagTexts = aTags.Select(aTag => aTag.InnerText.Trim());
//                    return string.Join(", ", tagTexts); // Join tag texts with comma separator
//                }
//            }
//        }

//        return "No tags found"; // Return this if no tags are found
//    }

//    // Function to save filtered URLs, headings, dates, judgment URLs, tags, and apply formatting
//    static void SaveUrlsToExcel(List<(string url, string category, string heading, string date, string judgmentUrl, string tags)> urlWithDetails, string filePath)
//    {
//        using (var workbook = new XLWorkbook())
//        {
//            var worksheet = workbook.Worksheets.Add("Filtered URLs");

//            // Add headers with capitalized first letters
//            worksheet.Cell(1, 1).Value = "S.No.";
//            worksheet.Cell(1, 2).Value = "Date";
//            worksheet.Cell(1, 3).Value = "Category";
//            worksheet.Cell(1, 4).Value = "Headline";
//            worksheet.Cell(1, 5).Value = "Tags";
//            worksheet.Cell(1, 6).Value = "Judgment URL";
//            worksheet.Cell(1, 7).Value = "URL";

//            // Set header row background color
//            var headerRange = worksheet.Range("A1:G1");
//            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
//            headerRange.Style.Font.Bold = true;

//            int row = 2;
//            foreach (var (url, category, heading, date, judgmentUrl, tags) in urlWithDetails)
//            {
//                worksheet.Cell(row, 1).Value = row - 1; // Serial number
//                worksheet.Cell(row, 2).Value = date;
//                worksheet.Cell(row, 3).Value = category;
//                worksheet.Cell(row, 4).Value = heading;
//                worksheet.Cell(row, 5).Value = tags;
//                worksheet.Cell(row, 6).Value = judgmentUrl;
//                worksheet.Cell(row, 7).Value = url;
//                row++;
//            }

//            // Add borders and alternate row colors
//            var tableRange = worksheet.Range($"A1:G{row - 1}");
//            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
//            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

//            // Apply alternate row colors
//            for (int i = 2; i < row; i++)
//            {
//                if (i % 2 == 0)
//                {
//                    worksheet.Row(i).Style.Fill.BackgroundColor = XLColor.LightBlue;
//                }
//                else
//                {
//                    worksheet.Row(i).Style.Fill.BackgroundColor = XLColor.White;
//                }
//            }
//            // Autofit columns for better visibility
//            worksheet.Columns().AdjustToContents();

//            // Save the file
//            workbook.SaveAs(filePath);
//        }
//    }
//}

/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ClosedXML.Excel;

class Program
{
    static async Task Main(string[] args)
    {
        string url = "https://www.barandbench.com/";

        // List of keywords to filter URLs
        List<string> keywords = new List<string> { "news", "columns", "law-firms", "interviews" };

        // Get the HTML content of the page
        var htmlContent = await GetHtmlAsync(url);

        if (!string.IsNullOrEmpty(htmlContent))
        {
            // Extract hrefs from all a tags
            List<string> hrefs = ExtractHrefs(htmlContent);

            // Filter the hrefs based on the keywords with pattern /keyword/ and ending with -numbers
            List<(string, string)> filteredHrefs = FilterUrlsByPatternAndNumber(hrefs, keywords);

            // Visit each filtered URL and extract the <h1> text, date, judgment URL, and tags
            var urlWithDetails = await GetDetailsFromUrls(filteredHrefs);

            // Save the filtered URLs, headings, dates, judgment URLs, and tags to an Excel file
            SaveUrlsToExcel(urlWithDetails, "FilteredUrlsWithHeadingsAndTags.xlsx");
        }
    }

    // Function to fetch the HTML content from a URL
    static async Task<string> GetHtmlAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string htmlContent = await response.Content.ReadAsStringAsync();
                return htmlContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching HTML content: {ex.Message}");
                return string.Empty;
            }
        }
    }

    // Function to extract all hrefs from a tags
    static List<string> ExtractHrefs(string htmlContent)
    {
        List<string> hrefList = new List<string>();
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        var aTags = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
        if (aTags != null)
        {
            foreach (var aTag in aTags)
            {
                string href = aTag.GetAttributeValue("href", string.Empty);
                if (!string.IsNullOrEmpty(href))
                {
                    hrefList.Add(href);
                }
            }
        }

        return hrefList;
    }

    // Function to filter URLs based on the pattern /keyword/ and ending with -numbers
    static List<(string, string)> FilterUrlsByPatternAndNumber(List<string> hrefs, List<string> keywords)
    {
        List<(string, string)> filteredHrefs = new List<(string, string)>();      
            
        foreach (var href in hrefs)
        {
            foreach (var keyword in keywords)
            {
                // Check if the URL contains the keyword in /keyword/ pattern and ends with -numbers
                //Regex.IsMatch("https://latestlaw.com/Latest-news/lkaldkalkdlakdla/adkjakdjadk", @"latest-news(/.*)?$", RegexOptions.IgnoreCase)
                //Regex.IsMatch(href, $@"{keyword}(/.*)?$", RegexOptions.IgnoreCase);
                if (Regex.IsMatch(href, $@"https?://[^/]+/{keyword}+[/^]", RegexOptions.IgnoreCase))
                
                {
                    filteredHrefs.Add((href, keyword)); // Add href and keyword as a tuple
                }
            }
        }

        return filteredHrefs.Distinct().ToList();
    }

    // Function to visit each URL, extract <h1> text, date, judgment URL, and tags
    static async Task<List<(string url, string category, string heading, string date, string judgmentUrl, string tags)>> GetDetailsFromUrls(List<(string url, string category)> filteredHrefs)
    {
        List<(string url, string category, string heading, string date, string judgmentUrl, string tags)> result = new List<(string url, string category, string heading, string date, string judgmentUrl, string tags)>();

        foreach (var (url, category) in filteredHrefs)
        {
            string fullUrl = AddUrlPrefix(url);  // Ensure proper URL format
            Console.WriteLine(fullUrl);
            string htmlContent = await GetHtmlAsync(fullUrl);
            await Task.Delay(500);
            string heading = await GetH1TextFromUrl(fullUrl, htmlContent);
            string date = await GetDateFromUrl(fullUrl, htmlContent);  // Extract date
            string judgmentUrl = await GetJudgmentUrl(fullUrl, htmlContent); // Extract judgment URL
            string tags = await GetTagsFromUrl(fullUrl, htmlContent);  // Extract tags
            result.Add((fullUrl, category, heading, date, judgmentUrl, tags));
        }

        return result;
    }

    // Function to add the correct URL prefix if necessary
    static string AddUrlPrefix(string url)
    {
        //if (!url.Contains("https://www.latestlaws.com/") && !url.Contains("hindi.latestlaws.cpm/"))
        //{
        //    return $"https://www.latestlaws.com/{url.TrimStart('/')}";
        //}

        return url;
    }

    // Function to extract the <h1> text from a URL
    static async Task<string> GetH1TextFromUrl(string url, string _htmlContent)
    {
        string htmlContent = _htmlContent;
        if (!string.IsNullOrEmpty(htmlContent))
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var h1Tag = htmlDoc.DocumentNode.SelectSingleNode("//h1");
            if (h1Tag != null)
            {
                return h1Tag.InnerText.Trim();
            }
        }

        return "No heading found"; // Return this if no <h1> tag is found
    }

    // Function to extract the date from a URL (return only the date, no time)
    static async Task<string> GetDateFromUrl(string url, string _htmlContent)
    {
        string htmlContent = _htmlContent;
        if (!string.IsNullOrEmpty(htmlContent))
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // Select the <time> element with the specified class
            var timeTag = htmlDoc.DocumentNode.SelectSingleNode("//time[@class='arr__timeago']");

            if (timeTag != null)
            {
                // Get the value of the 'datetime' attribute
                string dateTimeValue = timeTag.GetAttributeValue("datetime", null);

                // Parse the date and return it
                if (DateTime.TryParse(dateTimeValue, out DateTime parsedDate))
                {
                    return parsedDate.ToString("yyyy-MM-dd"); // Returns the parsed DateTime object
                }
            }
            else
            {
                return "No div found";
            }
        }

        return "No date found"; // Return this if no <p data-datestring> tag is found
    }

    // Function to extract the judgment URL from the page
    static async Task<string> GetJudgmentUrl(string url, string _htmlContent)
    {
        string htmlContent = _htmlContent;
        if (!string.IsNullOrEmpty(htmlContent))
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var judgmentLink = htmlDoc.DocumentNode.SelectSingleNode("//a[contains(text(), 'Click Here To Read/Download Judgment')]");
            if (judgmentLink != null)
            {
                return judgmentLink.GetAttributeValue("href", "No judgment URL found");
            }
        }

        return "No judgment URL found"; // Return this if no judgment link is found
    }

    // Function to extract the tags from the page
    static async Task<string> GetTagsFromUrl(string url, string _htmlContent)
    {
        string htmlContent = _htmlContent;
        if (!string.IsNullOrEmpty(htmlContent))
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var tagsDiv = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'arrow-component arr--story-tags')]");
            if (tagsDiv != null)
            {
                var aTags = tagsDiv.SelectNodes(".//a");
                if (aTags != null)
                {
                    var tagTexts = aTags.Select(aTag => aTag.InnerText.Trim());
                    return string.Join(", ", tagTexts); // Join tag texts with comma separator
                }
            }
        }

        return "No tags found"; // Return this if no tags are found
    }

    // Function to save filtered URLs, headings, dates, judgment URLs, tags, and apply formatting
    static void SaveUrlsToExcel(List<(string url, string category, string heading, string date, string judgmentUrl, string tags)> urlWithDetails, string filePath)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Filtered URLs");

            // Add headers with capitalized first letters
            worksheet.Cell(1, 1).Value = "S.No.";
            worksheet.Cell(1, 2).Value = "Date";
            worksheet.Cell(1, 3).Value = "Category";
            worksheet.Cell(1, 4).Value = "Headline";
            worksheet.Cell(1, 5).Value = "Tags";
            worksheet.Cell(1, 6).Value = "Judgment URL";
            worksheet.Cell(1, 7).Value = "URL";

            // Set header row background color
            var headerRange = worksheet.Range("A1:G1");
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Font.Bold = true;

            int row = 2;
            foreach (var (url, category, heading, date, judgmentUrl, tags) in urlWithDetails)
            {
                worksheet.Cell(row, 1).Value = row - 1; // Serial number
                worksheet.Cell(row, 2).Value = date;
                worksheet.Cell(row, 3).Value = category;
                worksheet.Cell(row, 4).Value = heading;
                worksheet.Cell(row, 5).Value = tags;
                worksheet.Cell(row, 6).Value = judgmentUrl;
                worksheet.Cell(row, 7).Value = url;
                row++;
            }

            // Add borders and alternate row colors
            var tableRange = worksheet.Range($"A1:G{row - 1}");
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Apply alternate row colors
            for (int i = 2; i < row; i++)
            {
                if (i % 2 == 0)
                {
                    worksheet.Row(i).Style.Fill.BackgroundColor = XLColor.LightBlue;
                }
                else
                {
                    worksheet.Row(i).Style.Fill.BackgroundColor = XLColor.White;
                }
            }
            // Autofit columns for better visibility
            worksheet.Columns().AdjustToContents();

            // Save the file
            workbook.SaveAs(filePath);
        }
    }
}
*/
using System;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

class Program
{
    static async Task Main(string[] args)
    {
        // Define the connection string for the SQL Server database
        //string connectionString = "Data Source=.;Initial Catalog=CRM_AVINO;persist security info=True;User ID=sa;Password=;TrustServerCertificate=True";
        string connectionString = "Data Source=sql8002.site4now.net;Initial Catalog=db_a94bc9_crm;persist security info=True;User ID=db_a94bc9_crm_admin;Password=AFvpc8943a;TrustServerCertificate=True";

        // Path to the text file that contains the links (URLs)
        string linksFilePath = "c:\\mylinks\\links.txt";

        try
        {
            // Read all the content from the text file
            string fileContent = File.ReadAllText(linksFilePath);

            // Split the content by commas and trim extra spaces and newlines
            string[] links = fileContent.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            
            //string[] finallinks = links.Distinct().ToArray();

            // Normalize links by trimming spaces and converting to lowercase
            string[] finallinks = links
                .Select(link => link.Trim().ToLower())  // Trim and convert to lowercase
                .Distinct()                            // Remove duplicates
                .ToArray();

            // Create a new SqlConnection object
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Create an HttpClient instance for downloading HTML content from URLs
                using (HttpClient client = new HttpClient())
                {
                    foreach (var link in finallinks)
                    {
                        var trimmedLink = link.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmedLink))
                        {
                            await ProcessUrl(trimmedLink, client, connection);
                            // Adding a delay of 2 seconds (2000 milliseconds) between each request
                            await Task.Delay(3000);  // Adjust this value as needed
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    private static readonly string[] UserAgents = new[]
    {
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Gecko/20100101 Firefox/89.0",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0.1 Safari/605.1.15",
        // Add more user agents if needed
    };

    // Method to process a URL and insert data into the database
    static async Task ProcessUrl(string url, HttpClient client, SqlConnection connection)
    {
        try
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgents[new Random().Next(UserAgents.Length)]);

            // Download HTML content from the URL
            var htmlContent = await client.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);  // Load the HTML content into HtmlAgilityPack

            // Extract category and question text from the HTML
            var spanNodeCategory = doc.DocumentNode.SelectSingleNode("//div[@id='main']//section[@id='breadcrumb']//li[2]/a");
            var spanNode = doc.DocumentNode.SelectSingleNode("//div[@id='main']//section[@id='breadcrumb']//li[3]/span");

            string categoryName = spanNodeCategory?.InnerText.Trim().Replace("Law Guide", "").Replace(" Legal Advice", "").Replace(" / ", " and ") ?? "N/A";
            string pageName = spanNode?.InnerText.Trim().Replace(":", "").Replace(" ", "-") ?? "N/A";
            string questionText = spanNode?.InnerText.Trim() ?? "N/A";

            // Check if the category already exists
            string categoryQuery = "SELECT Id FROM Categories WHERE CategoryName = @CategoryName";
            using (SqlCommand command = new SqlCommand(categoryQuery, connection))
            {
                command.Parameters.AddWithValue("@CategoryName", categoryName);
                Console.WriteLine($"Executing query: {categoryQuery} with values: CategoryName = {categoryName}");
                object categoryIdObject = command.ExecuteScalar();

                int categoryId = 0;
                if (categoryIdObject != DBNull.Value)
                {
                    categoryId = Convert.ToInt32(categoryIdObject);
                }

                if (categoryId == 0)
                {
                    // Insert the category if it doesn't exist
                    string insertCategoryQuery = "INSERT INTO Categories (CategoryName) OUTPUT INSERTED.Id VALUES (@CategoryName)";
                    using (SqlCommand insertCommand = new SqlCommand(insertCategoryQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@CategoryName", categoryName);
                        Console.WriteLine($"Executing query: {insertCategoryQuery} with values: CategoryName = {categoryName}");
                        categoryIdObject = insertCommand.ExecuteScalar();

                        if (categoryIdObject != DBNull.Value)
                        {
                            categoryId = Convert.ToInt32(categoryIdObject);
                        }
                    }
                }

                // Insert the question into the database
                string insertQuestionQuery = "INSERT INTO Questions (CategoryId, QuestionText, UserQuestionText) OUTPUT INSERTED.Id VALUES (@CategoryId, @QuestionText, @UserQuestionText)";
                using (SqlCommand insertQuestionCommand = new SqlCommand(insertQuestionQuery, connection))
                {
                    insertQuestionCommand.Parameters.AddWithValue("@CategoryId", categoryId);
                    insertQuestionCommand.Parameters.AddWithValue("@QuestionText", HttpUtility.HtmlEncode(questionText));
                    insertQuestionCommand.Parameters.AddWithValue("@UserQuestionText", HttpUtility.HtmlEncode(GetUserQuestionText(doc)));
                    Console.WriteLine($"Executing query: {insertQuestionQuery} with values: CategoryId = {categoryId}, QuestionText = {questionText}, UserQuestionText = {GetUserQuestionText(doc)}");

                    object questionIdObject = insertQuestionCommand.ExecuteScalar();

                    int questionId = 0;
                    if (questionIdObject != DBNull.Value)
                    {
                        questionId = Convert.ToInt32(questionIdObject);
                    }

                    // Extract answers from the HTML
                    var answerNodes = doc.DocumentNode.SelectNodes("//div[@class='answer-body']");

                    if (answerNodes != null)
                    {
                        int answerOrder = 1;
                        foreach (var answerNode in answerNodes)
                        {
                            var textNode = answerNode.SelectSingleNode("./div[@itemprop='text']");
                            if (textNode != null)
                            {
                                string answerText = textNode.InnerHtml.Replace("<br>", "\n").Trim();

                                // Insert the answer into the Answers table
                                string insertAnswerQuery = "INSERT INTO Answers (QuestionId, AnswerText, AnswerOrder) VALUES (@QuestionId, @AnswerText, @AnswerOrder)";
                                using (SqlCommand insertAnswerCommand = new SqlCommand(insertAnswerQuery, connection))
                                {
                                    insertAnswerCommand.Parameters.AddWithValue("@QuestionId", questionId);
                                    insertAnswerCommand.Parameters.AddWithValue("@AnswerText", HttpUtility.HtmlEncode(answerText));
                                    insertAnswerCommand.Parameters.AddWithValue("@AnswerOrder", answerOrder);
                                    Console.WriteLine($"Executing query: {insertAnswerQuery} with values: QuestionId = {questionId}, AnswerText = {answerText}, AnswerOrder = {answerOrder}");
                                    insertAnswerCommand.ExecuteNonQuery();
                                }

                                answerOrder++;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing URL {url}: {ex.Message}");
        }
    }

    // Method to extract user question text (if available)
    static string GetUserQuestionText(HtmlDocument doc)
    {
        var questionBodyNode = doc.DocumentNode.SelectSingleNode("//div[@class='question-body']/div[@itemprop='text']");
        if (questionBodyNode != null)
        {
            return questionBodyNode.InnerText.Trim();
        }
        else
        {
            return "N/A";
        }
    }
}
