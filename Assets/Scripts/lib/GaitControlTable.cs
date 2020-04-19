using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class GaitControlTable : MonoBehaviour {

    /// <summary>
    /// Заголовок таблицы. Содержит ссылки на объекты двигателей в заданном порядке.
    /// </summary>
    public List<Driver> header;

    public string folderPath = @"Resources/Gait Control Tables";

    /// <summary>
    /// Текущая строка таблицы.
    /// </summary>
    public int step = 0;

    /// <summary>
    /// Активно ли сейчас табличное управление.
    /// </summary>
    public bool inProgress = false;

    public bool isReversed = false; 

    public bool isKeyboardControlled = false;

    public bool isReady {
        get {
            return DriversAreReady();
        }
    }

    /// <summary>
    /// Управление таблицей с клавиатуры:
    /// -> для перехода к следующему шагу,
    /// <- для возврата к предыдущему.
    /// </summary>
    public bool keyboardControl = false;

    /// <summary>
    /// Список, в котором хранятся строки таблицы в формате 
    /// Dictionary <driverIndex, driverAction>.
    /// </summary>
    public List<Dictionary<int, DriverAction>> data;

    /// <summary>
    /// Добавить в таблицу управления строку, содержащую информацию о
    /// перемещении двигателей на заданном шаге. В скобках может быть
    /// указано время (в секундах), за которое нужно выполнить действие.
    /// В качестве разделителя должна быть использована запятая (,).
    /// В случае, если требуется относительное перемещение, перед значением
    /// следует указать один из двух операторов: "--" для движения
    /// в отрицательном направлении или "++" - в положительном.
    /// 
    /// </summary>
    /// <param name="line">
    /// Строка в следующем формате:
    /// 
    /// "<значение угла первого двигателя> [(время выполнения в с.)], 
    /// <значение угла второго двигателя> [(время выполнения в с.)],
    /// ... ,
    /// <значение угла i-го двигателя> [(время выполнения в с.)]"
    /// 
    /// В случае, если перемещение двигателя не требуется, значение следует
    /// пропустить, однако, оставить разделители. Если не требуется
    /// перемещение нескольких двигателей в конце заголовка,
    /// указание их значений можно опустить.
    /// 
    /// В КОНЦЕ СТРОК РАЗДЕЛИТЕЛЬ НЕ СТАВИТСЯ. ЗАПЯТАЯ БУДЕТ ИНТЕРПРЕТИРОВАНА
    /// КАК НАЛИЧИЕ ЕЩЕ ОДНОГО ПУСТОГО ЗНАЧЕНИЯ СТРОКИ ТАБЛИЦЫ.
    /// 
    /// Например, для таблицы с четырьмя элементами в заголовке:
    /// а) "45, 30, 15, -45"
    /// б) "0 (0.5),,, -90 (0.75)"
    /// в) ",,, -15 (1.5)"
    /// г) "++15, 0"
    /// д) "++30 (1.2), --60 (2.4), --90 (3.6), ++15 (0.6)"
    /// e) "++15, --15, 0, 60"
    /// </param>
    public void AddLine(string line)
    {
        var drvActions = new Dictionary<int, DriverAction>();
        int counter = 0;
        // Удаляем все пробелы из строки. Ихниктонелюбит:
        line = Regex.Replace (line, @"\s+|\t+", "");
        // Разбираем строку на элементы:
        string[] items = line.Split(',');
        // Проверяем, не больше ли количество элементов в строке,
        // чем указано двигателей в заголовке.
        if (items.Length > header.Count)
        {
            Debug.LogError (string.Format("Невозможно обработать строку \"{0}\", " +
                                          "поскольку количество элементов в ней ({1}) превышает длину заголовка ({2}).", line, items.Length, header.Count));
            return;
        }
        // Магия регулярных выражений:
        string pattern = @"^(([\-]{0,2}?|[\+]{0,2}?)[0-9]+(\.[0-9]+)?|" +
            @"(([0-9]+[\-,\+,\*,\/])?(max|min|default)([\-,\+,\*,\/][0-9]+)?))(\([0-9]+(\.[0-9]+)?\))?$";
        foreach (string item in items)
        {
            if (item != "")
            {
                if (!Regex.IsMatch(item, pattern))
                {
                    Debug.LogError(string.Format("Формат элемента \"{0}\" " +
                                                 "не соответствует шаблону, невозможно обработать строку \"{1}\".", item, line));
                    return;
                }
                var drvAction = new DriverAction(
                    // Проверить, задана ли величина угла в абсолютном или относительном виде:
                    !Regex.IsMatch(item, @"[\-]{2}?|[\+]{2}?"),
                    // Взять часть, соответствующую углу целиком:
                    ParseAngleValue(header[counter], item),
                    // Проверить, указано ли время работы двигателя:
                    Regex.IsMatch(item, @"\([0-9]+(\.[0-9]+)?\)$"),
                    // Взять только численную часть, соответствующую времени, обрезав скобки:
                    Regex.IsMatch(item, @"\([0-9]+(\.[0-9]+)?\)$") ? float.Parse(Regex.Match(item, @"\(([0-9]+(\.[0-9]+)?)\)$").Groups[1].Value) : 0
                );
                drvActions.Add(counter, drvAction);
                //print(string.Format("Is Absolute: {0}, value: {1}, isCustomTime: {2}, time: {3}", drvActions[counter].valueAbsolute,
                              // drvActions[counter].value, drvActions[counter].timeCustom, drvActions[counter].timeValue));
            }
            counter++;
        }
        AddLine(drvActions);
    }

    public void AddLine(Dictionary<int, DriverAction> line)
    {
        // Проверка того, находится ли заданное значение в пределах зоны работы двигателя.
        data.Add(line);
    }

    /// <summary>
    /// Обработка значения угла поворота двигателя, включая специальные символы.
    /// </summary>
    public float ParseAngleValue(Driver drv, string line)
    {
        // Проверка, содержит ли запись угла специальные символы.
       // if (!Regex.IsMatch(line, @"min|max|default"))
        //    return float.Parse(Regex.Match(line, @"([\-]?|[\+]?)([0-9]+(\.[0-9]+)?)").Groups[0].Value);
        // Если да, обработка специальных символов.


        string s = Regex.Match(line, @"([\-]?|[\+]?)([0-9]+(\.[0-9]+)?|" +
            @"(([0-9]+[\-,\+,\*,\/])?(max|min|default)([\-,\+,\*,\/][0-9]+)?))").Groups[0].Value;
        if (!Regex.IsMatch(s, @"^(max|min|default)$"))
        {
            s = Regex.Replace(s, @"max|min|default",
                              HandleSpecialSymbols(drv, Regex.Match(s, @"max|min|default").Groups[0].Value));

            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), s);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            s = (string)row["expression"];
        }
        else
            s = HandleSpecialSymbols(drv, s);
        return float.Parse(s);
    }

    /// <summary>
    /// Обработка таких символов как "max, min, default", а также выполнение математических операций.
    /// </summary>
    /// <returns>Числовой результат в виде float.</returns>
    /// <param name="drv">Двигатель, параметры которого необходимо использовать.</param>
    /// <param name="s">Строковое представление выражения.</param>
    public string HandleSpecialSymbols (Driver drv, string s) {
        switch (s)
        {
            case "max": s = drv.qMax.ToString(); break;
            case "min": s = drv.qMin.ToString(); break;
            case "default": s = drv.qStart.ToString(); break; 
                
            // С помощью Dictionary можно сделать проверку на коэффиценты.
        }
        return s;
    }

    public void Clear ()
    {
        data.Clear(); 
    }

    /// <summary>
    /// Класс, описывающий одно перемещение двигателя.
    /// Поля класса хранят информацию о том, является ли движение абсолютным 
    /// (установить двигатель в заданное положение) или относительным (добавить
    /// к текущему положению двигателя заданную величину), а также должно ли
    /// перемещение быть отработано в течение заданного времени.
    /// </summary>
    public class DriverAction {
        // Значение, которое необходимо отработать двигателю:
        // Если valueAbsolute == true (абсолютное значение), переместить двигатель в заданное положение,
        // иначе (относительное значение) добавить величину к текущему положению двигателя.
        public bool valueAbsolute;
        public float value;

        // Значение, описывающее скорость движения двигателя:
        // Если timeCustom == true, выполнить движение за указанное время,
        // иначе использовать известную скорость двигателя.
        public bool timeCustom;
        public float timeValue;

        public DriverAction (bool valueAbsolute, float value, bool timeCustom, float timeValue) {
            this.valueAbsolute = valueAbsolute;
            this.value = value;
            this.timeCustom = timeCustom;
            this.timeValue = timeValue;
        }
    }

    /// <summary>
    /// Установить заголовок таблицы.
    /// </summary>
    /// <param name="drvs">Список двигателей, в порядке, формирующем заголовок таблицы.</param>
    public void SetHeader(Driver[] drvs)
    {
        // Добавить првоерку, не добавляется ли один и тот же двигатель несколько раз.
        header = new List<Driver>(drvs);
        data = new List<Dictionary<int, DriverAction>>();
    }

    /// <summary>
    /// Построить таблицу из текстового файла.
    /// </summary>
    /// <param name="robot">Модель робота для получения элементов двигателей
    /// в соответствии с заголовком таблицы из файла.</param>
    /// <param name="path">Путь к файлу таблицы.</param>
    public void ReadFromFile (ModularRobot robot, string path) {
        // Проверяем, существует ли файл.
        //
        //
        //
        // ???
        string fullPath = Path.Combine(Path.Combine(Application.dataPath, folderPath), path);
        print(fullPath);
        StreamReader sr = new StreamReader(fullPath);
        // 1. Построить header. Очистить его.
        string headerString = Regex.Replace(sr.ReadLine(), @"\s+|\t+", "");
        print(headerString);
        headerString = Regex.Match(headerString, "header=\"([0-9a-z,_]+)*\"").Groups[1].Value;
        if (headerString == "" || headerString == null) {
            Debug.LogError(string.Format("Невозможно построить таблицу из файла {0}, не удалось прочитать header.", fullPath));
            return;
        }
        string[] headerElements = headerString.Split(',');
        header = new List<Driver>();
        var drivers = new List<Driver>();
        string hId, hDrv;
        foreach (string s in headerElements) {
            // ЗДЕСЬ МЫ ОБРАБАТЫВАЕМ ЗАГОЛОВОК ТАБЛИЦЫ: (ДОРАБОТАТЬ)
            if (Regex.IsMatch(s, @"([0-9.]+)(_([0-9a-z]+))*"))
            {
                hId = Regex.Match(s, @"([0-9.]+)(_([0-9a-z]+))*").Groups[1].Value;
                hDrv = Regex.Match(s, @"([0-9.]+)(_([0-9a-z]+))*").Groups[3].Value;
                if (robot.modules.ContainsKey(int.Parse(hId)))
                {
                    if (hDrv != "")
                        drivers.Add(robot.modules[int.Parse(hId)].drivers[hDrv]);
                    else
                    {
                        if (robot.modules[int.Parse(hId)].drivers.ContainsKey("q1"))
                            drivers.Add(robot.modules[int.Parse(hId)].drivers["q1"]);
                        else
                            drivers.Add(robot.modules[int.Parse(hId)].GetComponent<Driver>());
                    }
                }
            }
        }
        print("Length of drivers is " + drivers.Count);
        SetHeader(drivers.ToArray());
        // 2. Строим таблицу.
        var line = "";
        while (!sr.EndOfStream)
        {
            line = sr.ReadLine();
            print(line);
            AddLine(line);
        }
        sr.Close();

        if (isReversed)
            step = data.Count - 1;
        else
            step = 0;
    }

    /// <summary>
    /// Сохранить таблицу текущего вида в текстовый файл.
    /// </summary>
    /// <param name="path">Путь, по которому следует сохранить файл.</param>
    public void SaveToFile (string path)
    {

    }

    /// <summary>
    /// Parses the time value.
    /// </summary>
    public void ParseTimeValue()
    {
        // Учет коэффициентов?
    }

    public void AddLineAtIndex(int index)
    {

    }

    /// <summary>
    /// Move drivers as for next line of GCT.
    /// </summary>
    public void NextStep () {
        if (DriversAreReady())
        {
            if (!inProgress)
            {
                inProgress = true;
                StartCoroutine(PlayNextStep());
            }
            else
                Debug.LogError("Cannot start GCT while it's active.");
        }
        else
            Debug.LogError("Cannot start GCT because of drivers are already in work.");
    }

    /// <summary>
    /// Move drivers as for previous line of GCT.
    /// </summary>
    public void PreviousStep () {
        if (DriversAreReady())
        {
            if (!inProgress)
            {
                inProgress = true;
                StartCoroutine(PlayPreviousStep());
            }
            else
                Debug.LogError("Cannot start GCT while it's active.");
        }
        else
            Debug.LogError("Cannot start GCT because of drivers are already in work.");
    }

    IEnumerator PlayNextStep (bool isComposite = false) {
        int count = 0;
        DriverAction dAction;
        if (step >= data.Count)
            step = 0;
        else if (step < 0)
            step = data.Count - 1;
        print("Step #" + step);
        foreach (Driver drv in header)
        {
            if (data[step].ContainsKey(count))
            {
                dAction = data[step][count];
                if (dAction.valueAbsolute)
                {
                    if (dAction.timeCustom)
                        drv.Set(data[step][count].value, time: dAction.timeValue);
                    else
                        drv.Set(data[step][count].value);
                }
                else
                {
                    if (dAction.timeCustom)
                        drv.Rotate(data[step][count].value, time: dAction.timeValue);
                    else
                        drv.Rotate(data[step][count].value);
                }
            }
            count++;
        }
        while (!DriversAreReady())
            yield return new WaitForEndOfFrame();
        step++;
        if (!isComposite)
            inProgress = false;
    }

    IEnumerator PlayPreviousStep (bool isComposite = false) {
        int count = 0;
        DriverAction dAction;
        if (step >= data.Count)
            step = 0;
        else if (step < 0)
            step = data.Count - 1;
        print("Step #" + step);
        foreach (Driver drv in header)
        {
            if (data[step].ContainsKey(count))
            {
                dAction = data[step][count];
                if (dAction.valueAbsolute)
                {
                    if (dAction.timeCustom)
                        drv.Set(data[step][count].value, time: dAction.timeValue);
                    else
                        drv.Set(data[step][count].value);
                }
                else
                {
                    if (dAction.timeCustom)
                        drv.Rotate(data[step][count].value * (-1), time: dAction.timeValue);
                    else
                        drv.Rotate(data[step][count].value * (-1));
                }
            }
            count++;
        }
        while (!DriversAreReady())
            yield return new WaitForEndOfFrame();
        step--;
        if (!isComposite)
            inProgress = false;
    }


    /// <summary>
    /// Воспроизведение таблицы до конца или до заданной строки, если параметр не задан.
    /// Если требуется воспроизвести следующие N шагов из неизвестного положения
    /// следует использовать gct.Begin(gct.step + N).
    /// Воспроизведение завершится ПЕРЕД указанной строкой.
    /// </summary>
    public void Begin(int terminationStep = -1)
    {
        if (DriversAreReady())
        {
            if (!inProgress)
            {
                inProgress = true;
                StartCoroutine(Worker(terminationStep));
            }
            else
                Debug.LogError("Cannot start GCT while it's active.");
        }
        else
            Debug.LogError("Cannot start GCT. Drivers are not ready.");
    }

    /// <summary>
    /// Воспроизвести один раз, пока не закончится таблица.
    /// </summary>
    public void BeginTillEnd()
    {
        if (DriversAreReady())
        {
            if (!inProgress)
            {
                inProgress = true;
                StartCoroutine(Worker());
            }
            else
                Debug.LogError("Cannot start GCT while it's active.");
        }
        else
            Debug.LogError("Cannot start GCT. Drivers are not ready.");
    }

    public void BeginTillEnd(bool isReversed)
    {
        this.isReversed = isReversed;
        if (DriversAreReady())
        {
            if (!inProgress)
            {
                inProgress = true;
                StartCoroutine(Worker());
            }
            else
                Debug.LogError("Cannot start GCT while it's active.");
        }
        else
            Debug.LogError("Cannot start GCT. Drivers are not ready.");
    }

    /// <summary>
    /// Непрерывное воспроизведение определенного количества раз. Если параметр не указан, пока DCT не будет остановлен.
    /// </summary>
    /// <param name="times">Количество повторений.</param>
    public void BeginLoop(float times = Mathf.Infinity)
    {
        if (DriversAreReady())
        {
            if (!inProgress)
            {
                inProgress = true;
                StartCoroutine(WorkerLoop(times));
            }
            else
                Debug.LogError("Cannot start GCT while it's active.");
        }
        else
            Debug.LogError("Cannot start GCT. Drivers are not ready.");
    }

    /// <summary>
    /// Begin this instance. Пока не придет команда остановиться или до конкретного шага.
    /// </summary>
    public void BeginContinuously()
    {
        if (DriversAreReady())
        {
            if (!inProgress)
            {
                inProgress = true;
                StartCoroutine(WorkerWithWaiting());
            }
            else
                Debug.LogError("Cannot start GCT while it's active.");
        }
        else
            Debug.LogError("Cannot start GCT. Drivers are not ready.");
    }

    IEnumerator WorkerLoop(float times)
    {
        int count = 0;
        while (count < times)
        {
            inProgress = true;
            StartCoroutine(Worker());
            do
            {
                yield return new WaitForEndOfFrame();
            } while (inProgress);
            count++;
        }
    }

    IEnumerator WorkerWithWaiting()
    {
        while (inProgress)
        {
            if (!isReversed)
            {
                while (inProgress)
                {
                    if (step < data.Count)
                        StartCoroutine(PlayNextStep(isComposite: true));
                    do
                    {
                        yield return new WaitForEndOfFrame();
                    } while (!DriversAreReady());
                }
                step = 0;
            }
            else
            {
                Debug.LogError("Невозможно запустить воспроизведение таблицы" +
                               " с ожиданием новой строки в реверсивном (обратном) режиме.");
                inProgress = false;
            }
        }
    }

    IEnumerator Worker(int terminationStep = -1)
    {
        if (isReversed) {
            while (step >= 0 && inProgress && step != terminationStep)
            {
                StartCoroutine(PlayPreviousStep(isComposite: true));
                do
                {
                    yield return new WaitForEndOfFrame();
                } while (!DriversAreReady());
            }
            step = data.Count - 1;
        }
        else {
            while (step < data.Count && inProgress && step != terminationStep)
            {
                StartCoroutine(PlayNextStep(isComposite: true));
                do
                {
                    yield return new WaitForEndOfFrame();
                } while (!DriversAreReady());
            }
            step = 0;
        }
        inProgress = false;
    }

    private bool DriversAreReady()
    {
        foreach (Driver drv in header)
        {
            if (drv.busy)
                return false;
        }
        return true;
    }

    public void Pause()
    {
        inProgress = false;
    }

    public void ForcePause()
    {
        StopAllCoroutines();
        inProgress = false;
    }

    public void Stop()
    {
        inProgress = false;
        step = 0;
    }

    public void ForceStop()
    {
        StopAllCoroutines();
        inProgress = false;
        step = 0;
    }

    public void Reset()
    {
        ForceStop();
        header.Clear();
        data.Clear();
    }

    public void Destroy()
    {
        Destroy(this);
    }

    private void Update()
    {
        if (isKeyboardControlled && !inProgress)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
                NextStep();
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                PreviousStep();
        }
    }
}