namespace YandexDisk.Client.Protocol
{
    /// <summary>
    /// Возможные типы медиаданных
    /// </summary>
    public enum MediaType
    {
        /// <summary>
        ///  аудио-файлы.
        /// </summary>
        Audio,

        /// <summary>
        /// файлы резервных и временных копий.
        /// </summary>
        Backup,

        /// <summary>
        /// электронные книги.
        /// </summary>
        Book,

        /// <summary>
        /// сжатые и архивированные файлы.
        /// </summary>
        Compressed,

        /// <summary>
        /// файлы с базами данных.
        /// </summary>
        Data,

        /// <summary>
        /// файлы с кодом (C++, Java, XML и т. п.), а также служебные файлы IDE.
        /// </summary>
        Development,

        /// <summary>
        /// образы носителей информации в различных форматах и сопутствующие файлы (например, CUE).
        /// </summary>
        Diskimage,

        /// <summary>
        /// документы офисных форматов (Word, OpenOffice и т. п.).
        /// </summary>
        Document,

        /// <summary>
        /// зашифрованные файлы.
        /// </summary>
        Encoded,

        /// <summary>
        /// исполняемые файлы.
        /// </summary>
        Executable,

        /// <summary>
        /// файлы с флэш-видео или анимацией.
        /// </summary>
        Flash,

        /// <summary>
        /// файлы шрифтов.
        /// </summary>
        Font,

        /// <summary>
        /// изображения.
        /// </summary>
        Image,

        /// <summary>
        /// файлы настроек для различных программ.
        /// </summary>
        Settings,

        /// <summary>
        /// файлы офисных таблиц (Numbers, Lotus).
        /// </summary>
        Spreadsheet,

        /// <summary>
        /// текстовые файлы.
        /// </summary>
        Text,

        /// <summary>
        /// неизвестный тип.
        /// </summary>
        Unknown,

        /// <summary>
        /// видео-файлы.
        /// </summary>
        Video,

        /// <summary>
        /// различные файлы, используемые браузерами и сайтами (CSS, сертификаты, файлы закладок).
        /// </summary>
        Web
    }
}
