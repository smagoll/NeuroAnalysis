namespace Application.Services;

internal static class Prompts
{
    public const string ObjectDetection = """
                                          Ты система компьютерного зрения.

                                          Задача:
                                          Найди ВСЕ реальные физические объекты на изображении.

                                          Важно:
                                          - Только реальные предметы
                                          - Не сцена
                                          - Не фон
                                          - Не освещение
                                          - Не "room", "interior", "photo"

                                          Правила ответа:
                                          - Только существительные
                                          - На английском
                                          - Без пояснений
                                          - Без текста
                                          - Без markdown
                                          - Только JSON

                                          Верни СТРОГО JSON массив строк:

                                          ["object1","object2","object3"]

                                          Если объектов нет — верни [].
                                          """;

    public static string MaterialAnalysis(IEnumerable<string> objects) => $$$"""
                                                                             Ты эксперт по материалам.

                                                                             Дан список объектов:
                                                                             {{{string.Join(", ", objects)}}}

                                                                             Для КАЖДОГО объекта определи материалы изготовления.

                                                                             Правила:
                                                                             - Только материалы
                                                                             - Без описаний
                                                                             - Без комментариев
                                                                             - На английском
                                                                             - Только JSON
                                                                             - Не добавляй новые объекты

                                                                             Формат ответа:

                                                                             {
                                                                             "object": ["material1","material2"]
                                                                             }

                                                                             Пример:

                                                                             {
                                                                             "chair": ["wood","fabric"],
                                                                             "cup": ["ceramic"]
                                                                             }

                                                                             """;
}