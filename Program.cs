using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WinFormsApp {
  internal static class Program {
    [STAThread]
    private static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainForm());
    }
  }

  public class MainForm : Form {
    private readonly TabControl tabControl;

    // Задание 1
    private readonly TextBox txtCubeEdge;
    private readonly TextBox txtSphereRadius;
    private readonly TextBox txtResult;
    private readonly Button btnCalculate;

    // Задание 2
    private readonly TextBox txtGroup;
    private readonly TextBox txtGrade;
    private readonly TextBox txtStudentsResult;
    private readonly Button btnGenerate;
    private readonly Button btnFind;

    private readonly string studentsFilePath;

    public MainForm() {
      Text = "Практические задания";
      Size = new Size(700, 500);
      StartPosition = FormStartPosition.CenterScreen;

      studentsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "students.json");

      // Основной контейнер с вкладками
      tabControl = new TabControl {
        Location = new Point(10, 10),
        Size = new Size(665, 435)
      };

      // вкладдка 1
      TabPage cubeTab = new TabPage("Задание 1");

      Label lblA = new Label {
        Text = "Ребро куба (a):",
        Location = new Point(20, 20),
        Size = new Size(120, 20)
      };

      txtCubeEdge = new TextBox {
        Location = new Point(150, 20),
        Size = new Size(200, 20)
      };

      Label lblR = new Label {
        Text = "Радиус шара (R):",
        Location = new Point(20, 60),
        Size = new Size(120, 20)
      };

      txtSphereRadius = new TextBox {
        Location = new Point(150, 60),
        Size = new Size(200, 20)
      };

      btnCalculate = new Button {
        Text = "Вычислить",
        Location = new Point(150, 100),
        Size = new Size(200, 30)
      };

      btnCalculate.Click += BtnCalculate_Click;

      txtResult = new TextBox {
        Location = new Point(20, 150),
        Size = new Size(500, 130),
        Multiline = true,
        ReadOnly = true,
        ScrollBars = ScrollBars.Vertical
      };

      cubeTab.Controls.Add(lblA);
      cubeTab.Controls.Add(txtCubeEdge);
      cubeTab.Controls.Add(lblR);
      cubeTab.Controls.Add(txtSphereRadius);
      cubeTab.Controls.Add(btnCalculate);
      cubeTab.Controls.Add(txtResult);

      // Вкладка 2
      TabPage studentsTab = new TabPage("Задание 2");

      Label lblInfo = new Label {
        Text = "Работа с данными студентов (students.json)",
        Location = new Point(20, 20),
        Size = new Size(450, 20)
      };

      btnGenerate = new Button {
        Text = "Сгенерировать данные",
        Location = new Point(20, 55),
        Size = new Size(200, 30)
      };

      btnGenerate.Click += BtnGenerate_Click;

      Label lblGroup = new Label {
        Text = "Группа:",
        Location = new Point(20, 110),
        Size = new Size(100, 20)
      };

      txtGroup = new TextBox {
        Location = new Point(120, 107),
        Size = new Size(180, 20)
      };

      Label lblGrade = new Label {
        Text = "Оценка:",
        Location = new Point(20, 145),
        Size = new Size(100, 20)
      };

      txtGrade = new TextBox {
        Location = new Point(120, 142),
        Size = new Size(180, 20)
      };

      btnFind = new Button {
        Text = "Найти студентов",
        Location = new Point(330, 105),
        Size = new Size(190, 60)
      };

      btnFind.Click += BtnFind_Click;

      txtStudentsResult = new TextBox {
        Location = new Point(20, 190),
        Size = new Size(600, 165),
        Multiline = true,
        ReadOnly = true,
        ScrollBars = ScrollBars.Vertical
      };

      studentsTab.Controls.Add(lblInfo);
      studentsTab.Controls.Add(btnGenerate);
      studentsTab.Controls.Add(lblGroup);
      studentsTab.Controls.Add(txtGroup);
      studentsTab.Controls.Add(lblGrade);
      studentsTab.Controls.Add(txtGrade);
      studentsTab.Controls.Add(btnFind);
      studentsTab.Controls.Add(txtStudentsResult);

      // Добавление вкладок
      tabControl.TabPages.Add(cubeTab);
      tabControl.TabPages.Add(studentsTab);
      Controls.Add(tabControl);
    }

    // Задание 1
    private void BtnCalculate_Click(object sender, EventArgs e) {
      if (!double.TryParse(txtCubeEdge.Text, out double cubeEdge) || cubeEdge <= 0) {
        _ = MessageBox.Show("Введите корректное положительное число для ребра куба а");
        return;
      }

      if (!double.TryParse(txtSphereRadius.Text, out double sphereRadius) || sphereRadius <= 0) {
        _ = MessageBox.Show("Введите корректное положительное число для радиуса шара R");
        return;
      }

      if (sphereRadius > cubeEdge / 2.0) {
        _ = MessageBox.Show("Ошибка: Радиус шара R не может быть больше половины ребра куба (R <= a/2)");
        return;
      }

      double cubeVolume = Math.Pow(cubeEdge, 3);
      double sphereVolume = 4.0 / 3.0 * Math.PI * Math.Pow(sphereRadius, 3);
      double wasteVolume = cubeVolume - sphereVolume;
      double wastePercentage = wasteVolume / cubeVolume * 100;

      txtResult.Text = $"Объем куба (V куба): {cubeVolume:F3}\r\n" +
                       $"Объем шара (V шара): {sphereVolume:F3}\r\n" +
                       $"Отходы материала: {wastePercentage:F2}%";
    }

    // Задание 2
    private void BtnGenerate_Click(object sender, EventArgs e) {
      List<Student> students = GenerateStudents();
      string json = JsonConvert.SerializeObject(students, Formatting.Indented);

      try {
        File.WriteAllText(studentsFilePath, json);

        txtStudentsResult.Text = $"Данные успешно сгенерированы\r\n" +
                                 $"Количество студентов: {students.Count}\r\n\r\n" +
                                 $"Файл сохранен:\r\n{studentsFilePath}";
      }
      catch (Exception ex) {
        _ = MessageBox.Show($"Ошибка при сохранении файла:\r\n{ex.Message}");
      }
    }

    private void BtnFind_Click(object sender, EventArgs e) {
      if (!File.Exists(studentsFilePath)) {
        _ = MessageBox.Show("Файл students.json не найден\r\n" +
                            "Сначала нажмите «Сгенерировать данные»");
        return;
      }

      if (string.IsNullOrWhiteSpace(txtGroup.Text)) {
        _ = MessageBox.Show("Введите группу.");
        return;
      }

      if (!int.TryParse(txtGrade.Text, out int grade) || grade < 2 || grade > 5) {
        _ = MessageBox.Show("Оценка должна быть целым числом от 2 до 5");
        return;
      }

      try {
        string json = File.ReadAllText(studentsFilePath);
        List<Student> students = JsonConvert.DeserializeObject<List<Student>>(json);

        if (students == null) {
          _ = MessageBox.Show("Не удалось прочитать данные из файла");
          return;
        }

        string group = txtGroup.Text.Trim();
        List<Student> matchingStudents = students.Where(
          student => string.Equals(
            student.Group, group, StringComparison.OrdinalIgnoreCase)
            && student.Grade == grade)
          .ToList();

        txtStudentsResult.Clear();

        if (matchingStudents.Count == 0) {
          txtStudentsResult.Text = "Студенты с указанной группой и оценкой не найдены";
          return;
        }

        foreach (Student student in matchingStudents) {
          txtStudentsResult.AppendText($"{student.LastName} {student.FirstName} — " +
                                       $"группа: {student.Group}, оценка: {student.Grade}\r\n");
        }

        txtStudentsResult.AppendText($"\r\nНайдено студентов: {matchingStudents.Count}");
      }
      catch (Exception ex) {
        _ = MessageBox.Show($"Ошибка при чтении файла:\r\n{ex.Message}");
      }
    }

    // Генерация исходных данных
    private static List<Student> GenerateStudents() {
      string[] lastNames =
      {
        "Иванов",
        "Петров",
        "Сидоров",
        "Смирнов",
        "Кузнецов",
        "Попов",
        "Васильев",
        "Соколов",
        "Морозов",
        "Новиков",
        "Фёдоров",
        "Волков"
      };

      string[] firstNames =
      {
        "Жора",
        "Алексей",
        "Санёк",
        "Максим",
        "Артём",
        "Никитка",
        "Андрей",
        "Егор",
        "Никита",
        "Роман",
        "Стэпан",
        "Даня"
      };

      string[] groups =
      {
        "ПИ-21",
        "ПИ-22",
        "ПИ-23",
        "ПИ-24"
      };

      Random random = new Random();
      List<Student> students = new List<Student>();

      for (int studentIndex = 0; studentIndex < 20; ++studentIndex) {
        students.Add(new Student {
          LastName = lastNames[random.Next(lastNames.Length)],
          FirstName = firstNames[random.Next(firstNames.Length)],
          Group = groups[random.Next(groups.Length)],
          Grade = random.Next(2, 6)
        });
      }

      return students;
    }
  }

  // Модель данных студента
  internal class Student {
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Group { get; set; }
    public int Grade { get; set; }
  }
}