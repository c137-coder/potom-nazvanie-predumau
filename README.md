# Leo Game

2D платформер/экшен для Steam (Windows, macOS).

## Стек технологий

- **Движок:** Unity
- **Язык:** C#
- **Платформа:** Windows, macOS (Steam)
- **Арт:** placeholder-графика на старте, финальный арт — позже
- **VCS:** Git + Git LFS (текстуры, аудио, модели, бинарники)

## Структура репозитория

Файлы Unity-проекта (`Assets/`, `Packages/`, `ProjectSettings/` и т.д.) создаются через Unity Hub / Unity Editor прямо в этой папке — репозиторий уже настроен под них (`.gitignore`, `.gitattributes` с Git LFS).

## Как начать

1. Установить [Unity Hub](https://unity.com/download) и нужную версию редактора (рекомендуется актуальный LTS).
2. В Unity Hub: **New Project** → шаблон **2D (Core)** → указать путь к этой папке (`leo-game`).
3. После создания проекта закоммитить сгенерированные файлы:
   ```
   git add .
   git commit -m "Add Unity project scaffold"
   ```

## Git LFS

Git LFS уже инициализирован и настроен (`.gitattributes`) для бинарных ассетов Unity: текстуры, звук, видео, модели, шрифты, DLL. Убедиться, что LFS активен, можно командой:
```
git lfs status
```

## Сборка

В Unity Hub нужно поставить модули **Windows Build Support (Mono)** и **Mac Build Support (IL2CPP)** для установленной версии редактора (Editor → Installs → ⚙ → Add Modules).

Сборка из редактора: **File → Build Settings** → выбрать платформу (Windows / macOS) → **Build**.

Сборка из командной строки (используется `Assets/Editor/BuildScript.cs`):
```
# macOS
"/path/to/Unity" -batchmode -quit -projectPath . -executeMethod BuildScript.BuildMacOS -logFile -

# Windows
"/path/to/Unity" -batchmode -quit -projectPath . -executeMethod BuildScript.BuildWindows -logFile -
```
Готовые сборки попадают в `Builds/` (в `.gitignore`, не коммитятся).
