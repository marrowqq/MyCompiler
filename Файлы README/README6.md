**Лабораторная работа 7. Построение AST и проверка контекстно-зависимых условий**

**Цель работы:**
Изучить назначение и принципы работы семантического анализатора в структуре компилятора. Освоить методы построения абстрактного синтаксического дерева (AST) и проверки контекстно-зависимых условий (семантических правил) для заданной синтаксической конструкции.

**Сведения об авторе:**
Студент: Кашин Андрей Вячеславович Группа: АВТ-314

**Получение AST**

# Лабораторная работа 7. Анализ и преобразование кода с использованием Clang и LLVM
### Постановка задачи
1. Установка среды: Установить Clang, LLVM, opt и Graphviz (например, в Ubuntu 26.04).
2. Работа с AST: Сгенерировать абстрактное синтаксическое дерево для заданного C/C++‑файла.
3. Генерация LLVM IR: Получить промежуточное представление кода без оптимизаций (-O0) и с оптимизациями (-O2).
4. Оптимизация IR: Применить оптимизации с помощью opt и/или флагов Clang, сравнить изменения.
5. Построение CFG: Построить граф потока управления для одной или нескольких функций.
6. Индивидуальное задание (по варианту): Выполнить анализ конкретной синтаксической конструкции в соответствии с вариантом. Сформулировать, как LLVM обрабатывает выбранную конструкцию, какие оптимизации применяются.
Выводы
7. Ответить на контрольные вопросы
## Общее задание
### Исходный код
```
#include <stdio.h>

int square(int x)
{
    return x * x;
}

int main()
{
    int a = 5;
    int b = square(a);
    printf("%d\n", b);
    return 0;
}
```
### Получение AST
Команда:
```
clang -Xclang -ast-dump -fsyntax-only main.c
```
Результат: <br> <br>
<img width="1074" height="533" alt="image" src="https://github.com/user-attachments/assets/d6e6b11e-0d11-4599-b011-001dcb72b567">
### Генерация LLVM IR
Команда:
```
clang -S -emit-llvm main.c -o main.ll
```
Результат (main.ll):
```
; ModuleID = 'main.c'
source_filename = "main.c"
target datalayout = "e-m:e-p270:32:32-p271:32:32-p272:64:64-i64:64-i128:128-f80:128-n8:16:32:64-S128"
target triple = "x86_64-pc-linux-gnu"

@.str = private unnamed_addr constant [4 x i8] c"%d\0A\00", align 1

; Function Attrs: mustprogress nofree norecurse nosync nounwind willreturn memory(none) uwtable
define dso_local i32 @square(i32 noundef %0) local_unnamed_addr #0 {
  %2 = mul nsw i32 %0, %0
  ret i32 %2
}

; Function Attrs: nofree nounwind uwtable
define dso_local noundef i32 @main() local_unnamed_addr #1 {
  %1 = tail call i32 (ptr, ...) @printf(ptr noundef nonnull dereferenceable(1) @.str, i32 noundef 25)
  ret i32 0
}

; Function Attrs: nofree nounwind
declare noundef i32 @printf(ptr nocapture noundef readonly, ...) local_unnamed_addr #2

attributes #0 = { mustprogress nofree norecurse nosync nounwind willreturn memory(none) uwtable "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
attributes #1 = { nofree nounwind uwtable "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
attributes #2 = { nofree nounwind "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }

!llvm.module.flags = !{!0, !1, !2, !3}
!llvm.ident = !{!4}

!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 8, !"PIC Level", i32 2}
!2 = !{i32 7, !"PIE Level", i32 2}
!3 = !{i32 7, !"uwtable", i32 2}
!4 = !{!"Ubuntu clang version 18.1.3 (1ubuntu1)"}
```
### Оптимизация IR
Оптимизация -O0:
```
clang -O0 -S -emit-llvm main.c -o main_O0.ll
```
Результат (main_O0.ll):
```
; ModuleID = 'main.c'
source_filename = "main.c"
target datalayout = "e-m:e-p270:32:32-p271:32:32-p272:64:64-i64:64-i128:128-f80:128-n8:16:32:64-S128"
target triple = "x86_64-pc-linux-gnu"

@.str = private unnamed_addr constant [4 x i8] c"%d\0A\00", align 1

; Function Attrs: mustprogress nofree norecurse nosync nounwind willreturn memory(none) uwtable
define dso_local i32 @square(i32 noundef %0) local_unnamed_addr #0 {
  %2 = mul nsw i32 %0, %0
  ret i32 %2
}

; Function Attrs: nofree nounwind uwtable
define dso_local noundef i32 @main() local_unnamed_addr #1 {
  %1 = tail call i32 (ptr, ...) @printf(ptr noundef nonnull dereferenceable(1) @.str, i32 noundef 25)
  ret i32 0
}

; Function Attrs: nofree nounwind
declare noundef i32 @printf(ptr nocapture noundef readonly, ...) local_unnamed_addr #2

attributes #0 = { mustprogress nofree norecurse nosync nounwind willreturn memory(none) uwtable "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
attributes #1 = { nofree nounwind uwtable "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
attributes #2 = { nofree nounwind "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }

!llvm.module.flags = !{!0, !1, !2, !3}
!llvm.ident = !{!4}

!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 8, !"PIC Level", i32 2}
!2 = !{i32 7, !"PIE Level", i32 2}
!3 = !{i32 7, !"uwtable", i32 2}
!4 = !{!"Ubuntu clang version 18.1.3 (1ubuntu1)"}
```
Оптимизация O2:
```
clang -O2 -S -emit-llvm main.c -o main_O2.ll
```
Результат (main_O2.ll):
```
; ModuleID = 'main.c'
source_filename = "main.c"
target datalayout = "e-m:e-p270:32:32-p271:32:32-p272:64:64-i64:64-i128:128-f80:128-n8:16:32:64-S128"
target triple = "x86_64-pc-linux-gnu"

@.str = private unnamed_addr constant [4 x i8] c"%d\0A\00", align 1

; Function Attrs: mustprogress nofree norecurse nosync nounwind willreturn memory(none) uwtable
define dso_local i32 @square(i32 noundef %0) local_unnamed_addr #0 {
  %2 = mul nsw i32 %0, %0
  ret i32 %2
}

; Function Attrs: nofree nounwind uwtable
define dso_local noundef i32 @main() local_unnamed_addr #1 {
  %1 = tail call i32 (ptr, ...) @printf(ptr noundef nonnull dereferenceable(1) @.str, i32 noundef 25)
  ret i32 0
}

; Function Attrs: nofree nounwind
declare noundef i32 @printf(ptr nocapture noundef readonly, ...) local_unnamed_addr #2

attributes #0 = { mustprogress nofree norecurse nosync nounwind willreturn memory(none) uwtable "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
attributes #1 = { nofree nounwind uwtable "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
attributes #2 = { nofree nounwind "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }

!llvm.module.flags = !{!0, !1, !2, !3}
!llvm.ident = !{!4}

!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 8, !"PIC Level", i32 2}
!2 = !{i32 7, !"PIE Level", i32 2}
!3 = !{i32 7, !"uwtable", i32 2}
!4 = !{!"Ubuntu clang version 18.1.3 (1ubuntu1)"}
```
### Сравнение O0 и O2
Команда: 
```
diff main_O0.ll main_O2.ll
```
Результат:<br><br>
<img width="1729" height="953" alt="image" src="https://github.com/user-attachments/assets/f778b046-269d-453b-ae54-77a35119e408">
### Граф потока управления программы. Построение CFG
Команда для генерации оптимизированного LLVM IR:
```
clang -O2 -S -emit-llvm main.c -o main.ll
```
Команда для генерации .dot-файлов CFG для функций:
```
opt -dot-cfg -disable-output main.ll
```
Результат (.main.dot):
```
digraph "CFG for 'main' function" {
	label="CFG for 'main' function";

	Node0x557bf1b56630 [shape=record,color="#b70d28ff", style=filled, fillcolor="#b70d2870" fontname="Courier",label="{0:\l|  %1 = tail call i32 (ptr, ...) @printf(ptr noundef nonnull\l... dereferenceable(1) @.str, i32 noundef 25)\l  ret i32 0\l}"];
}
```
Результат (.square.dot):
```
digraph "CFG for 'square' function" {
	label="CFG for 'square' function";

	Node0x557bf1b54710 [shape=record,color="#b70d28ff", style=filled, fillcolor="#b70d2870" fontname="Courier",label="{1:\l|  %2 = mul nsw i32 %0, %0\l  ret i32 %2\l}"];
}
}
```
Команды для преобразования файлов с расширением .dot в .png с
помощью Graphviz:
```
dot -Tpng .main.dot -o cfg_main.png
dot -Tpng .square.dot -o cfg_square.png
```
Результат (main.cfg): <br><br>
<img width="660" height="144" alt="image" src="https://github.com/user-attachments/assets/1c3511fa-4f9d-4ce8-8644-fa4667101bb4"><br><br>
Результат (square.cfg): <br><br>
<img width="285" height="124" alt="image" src="https://github.com/user-attachments/assets/127f9449-3588-4c54-8b9a-c72a4d87d94c"><br>




**Индивидуальное задание**
Получение IR для -O0 и -O2
IR без оптимизации, команда:
```
clang -S -emit-llvm -O0 -Xclang -disable-O0-optnone main.cpp -o main_O0.ll
```
Результат:
<img width="1171" height="764" alt="image" src="https://github.com/user-attachments/assets/60d41116-a26a-4cf8-a41f-c1deb8c5acdb" />

**Генерация LLVM IR**
Команда:
```
clang -S -emit-llvm -O0 -Xclang -disable-O0-optnone main.cpp -o main_O0.ll
```
Результат(main_O0.ll):
```
; ModuleID = 'main.cpp'
source_filename = "main.cpp"
target datalayout = "e-m:w-p270:32:32-p271:32:32-p272:64:64-i64:64-i128:128-f80:128-n8:16:32:64-S128"
target triple = "x86_64-pc-windows-msvc19.33.0"

; Function Attrs: mustprogress noinline norecurse nounwind uwtable
define dso_local noundef i32 @main() #0 {
  %1 = alloca i32, align 4
  %2 = alloca i32, align 4
  %3 = alloca i32, align 4
  store i32 0, ptr %1, align 4
  store i32 0, ptr %2, align 4
  store i32 0, ptr %3, align 4
  br label %4

4:                                                ; preds = %7, %0
  %5 = load i32, ptr %2, align 4
  %6 = icmp slt i32 %5, 10
  br i1 %6, label %7, label %13

7:                                                ; preds = %4
  %8 = load i32, ptr %2, align 4
  %9 = load i32, ptr %3, align 4
  %10 = add nsw i32 %9, %8
  store i32 %10, ptr %3, align 4
  %11 = load i32, ptr %2, align 4
  %12 = add nsw i32 %11, 1
  store i32 %12, ptr %2, align 4
  br label %4, !llvm.loop !8

13:                                               ; preds = %4
  %14 = load i32, ptr %3, align 4
  ret i32 %14
}

attributes #0 = { mustprogress noinline norecurse nounwind uwtable "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }

!llvm.dbg.cu = !{!0}
!llvm.module.flags = !{!2, !3, !4, !5, !6}
!llvm.ident = !{!7}

!0 = distinct !DICompileUnit(language: DW_LANG_C_plus_plus_14, file: !1, producer: "clang version 22.1.6 (https://github.com/llvm/llvm-project fc4aad7b5db3fff421df9a9637605b9ca5667881)", isOptimized: false, runtimeVersion: 0, emissionKind: NoDebug, splitDebugInlining: false, nameTableKind: None)
!1 = !DIFile(filename: "main.cpp", directory: "C:\\Users\\user\\Desktop\\asdf")
!2 = !{i32 2, !"Debug Info Version", i32 3}
!3 = !{i32 1, !"wchar_size", i32 2}
!4 = !{i32 8, !"PIC Level", i32 2}
!5 = !{i32 7, !"uwtable", i32 2}
!6 = !{i32 1, !"MaxTLSAlign", i32 65536}
!7 = !{!"clang version 22.1.6 (https://github.com/llvm/llvm-project fc4aad7b5db3fff421df9a9637605b9ca5667881)"}
!8 = distinct !{!8, !9}
!9 = !{!"llvm.loop.mustprogress"}

```

Команда:
```
clang -S -emit-llvm -O2 main.cpp -o main_O2.ll
```
Результат(main_O2.ll):
```
; ModuleID = 'main.cpp'
source_filename = "main.cpp"
target datalayout = "e-m:w-p270:32:32-p271:32:32-p272:64:64-i64:64-i128:128-f80:128-n8:16:32:64-S128"
target triple = "x86_64-pc-windows-msvc19.33.0"

; Function Attrs: mustprogress nofree norecurse nosync nounwind willreturn memory(none) uwtable
define dso_local noundef i32 @main() local_unnamed_addr #0 {
  ret i32 45
}

attributes #0 = { mustprogress nofree norecurse nosync nounwind willreturn memory(none) uwtable "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cmov,+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }

!llvm.dbg.cu = !{!0}
!llvm.module.flags = !{!2, !3, !4, !5, !6}
!llvm.ident = !{!7}

!0 = distinct !DICompileUnit(language: DW_LANG_C_plus_plus_14, file: !1, producer: "clang version 22.1.6 (https://github.com/llvm/llvm-project fc4aad7b5db3fff421df9a9637605b9ca5667881)", isOptimized: true, runtimeVersion: 0, emissionKind: NoDebug, splitDebugInlining: false, nameTableKind: None)
!1 = !DIFile(filename: "main.cpp", directory: "C:\\Users\\user\\Desktop\\asdf")
!2 = !{i32 2, !"Debug Info Version", i32 3}
!3 = !{i32 1, !"wchar_size", i32 2}
!4 = !{i32 8, !"PIC Level", i32 2}
!5 = !{i32 7, !"uwtable", i32 2}
!6 = !{i32 1, !"MaxTLSAlign", i32 65536}
!7 = !{!"clang version 22.1.6 (https://github.com/llvm/llvm-project fc4aad7b5db3fff421df9a9637605b9ca5667881)"}

```
**Исследуйте, разворачивается ли цикл (-unroll)**
Цикл был полностью развернут и удален (Full Unrolling). Поскольку количество итераций ($10$) и начальные значения счетчика известны на этапе компиляции, оптимизатор полностью избавился от базовых блоков проверки условия и переходов, провел полную свертку констант (вычислил сумму $0 + 1 + 2 + \dots + 9 = 45$) и заменил всё тело функции main единственной инструкцией ret i32 45.

**Постройте CFG для main с -O0 и с -O2**
Команда:
```

```
Результат (-O0):

Результат (-O2):

Примените дополнительно -loop-rotate, -licm и опишите изменения.
Команда:
```

```
Результат (main_opt.ll):
```

```

Результат (cfg_opt.png):


Результат трансформации структуры цикла:
При применении прохода -loop-rotate компилятор преобразует цикл while в структуру do-while. Перед циклом создается специальный базовый блок-заголовок (Guard/Preheader), где условие проверяется один раз перед входом. Если проверка успешна, управление передается в тело цикла.
Изменения в CFG:
Внутри самого тела цикла проверка переносится в самый конец (в хвост блока body). Это позволяет убрать один лишний безусловный переход br на каждой итерации, так как условный переход сразу осуществляет возврат на новую итерацию, что снижает нагрузку на конвейер процессора и оптимизирует переходы.
Вывод - какие оптимизации применились к циклу?
- К переменным и памяти (-mem2reg): Все локальные переменные i и sum были подняты из медленной стековой памяти (alloca, load, store) в быстрые виртуальные SSA-регистры с использованием $\phi$-узлов.
- К циклу (-loop-unroll / -indvars): Переменная i была распознана как каноническая индукционная переменная. Благодаря фиксированным границам цикла ($i < 10$), компилятор произвел полное развертывание (Full Unroll).
- К вычислениям (Constant Folding): После развертывания цепочка сложений была вычислена компилятором превентивно, что свело всю функцию к возврату готового ответа 45.

**Дополнительное задание**
Выбранная синтаксическая конструкция
В качестве целевой конструкции используется Цикл while с предопределенным счетчиком и операциями инкремента:
```
while (i < 10) { i++; };
```
Построение абстрактного синтаксического дерева (AST)
Синтаксический анализ и построение AST были реализованы в рамках Лабораторной работы №5. Для целевой конструкции парсер формирует узел WhileNode, содержащий дочерний узел условия BinaryOpNode (со знаками <, >) и список выражений внутри тела цикла Body (UpdateNode для i++). Структура дерева визуализируется в графическом интерфейсе программы.

Генерация промежуточного представления (IR)
Исходное AST транслируется в трехадресный код виртуальных инструкций.
Набор используемых инструкций:
ALLOC_VAR — выделение памяти под переменную счетчика на виртуальном стеке.
COMPARE — вычисление логического выражения (условия выхода из цикла).
JUMP_IF_FALSE — условный переход по результату сравнения на блок выхода.
INCREMENT — атомарное изменение значения переменной на единицу.

Локальные оптимизации

Оптимизация 1: Свертка констант в условиях (Constant Folding)
Описание: Если в условии цикла используются две заведомо известные константы (например, while (5 < 2)), данный проход вычисляет результат выражения на этапе компиляции.
Сохранение семантики: Логическое выражение заменяется на константное значение false, превращая весь цикл в «мертвый код».

Оптимизация 2: Удаление недостижимого кода (Dead Code Elimination)
Описание: Если оптимизация условий (Оптимизация 1) определила, что условие цикла всегда ложно при первом входе, весь базовый блок тела цикла WhileLoop признается недостижимым и полностью вырезается из итогового списка инструкций IR.
Сохранение семантики: Код, который никогда не выполнится, удаляется, уменьшая размер результирующего файла без изменения логики работы программы.

Скриншоты работы программы

**Ответы на контрольные вопросы**
Что такое Clang, и какова его роль в процессе компиляции программ?
Ответ: Clang — это фронтенд-компилятор для языков семейства C, ответственный за разбор исходного кода, проверку синтаксиса и семантический анализ. Его главная роль заключается в превращении высокоуровневого кода, написанного человеком, в низкоуровневое промежуточное представление LLVM IR, понятное оптимизатору.

Что представляет собой LLVM и как он используется в современных компиляторах?
Ответ: LLVM — это универсальная модульная инфраструктура для создания компиляторов, которая предоставляет общие инструменты для оптимизации кода и генерации машинных инструкций под разные процессоры. Современные компиляторы используют LLVM как мощный «бэкенд», что позволяет им поддерживать множество архитектур (x86, ARM, RISC-V), не переписывая систему оптимизации с нуля.

Чем отличается абстрактное синтаксическое дерево (AST) от промежуточного представления LLVM IR?
Ответ: AST отражает иерархическую структуру исходного текста и сильно привязано к правилам конкретного языка программирования, сохраняя много лишних синтаксических деталей. LLVM IR — это линейное, машинно-независимое представление, которое больше похоже на ассемблер и предназначено для математического анализа и трансформаций перед генерацией кода.

Для чего необходимо промежуточное представление (IR) в процессе компиляции?
Ответ: Промежуточное представление служит универсальным «общим языком», который позволяет отделить анализ исходного кода (фронтенд) от генерации под конкретное железо (бэкенд). Благодаря IR можно один раз написать сложную оптимизацию и применять её для любого входного языка, будь то C++, Rust или Swift.

Что делает инструкция alloca в LLVM IR, и зачем она используется в функциях?
Ответ: Инструкция alloca выделяет память для переменной непосредственно в стековом кадре текущей функции. Она используется для работы с локальными переменными, позволяя обращаться к ним по адресу памяти до того, как оптимизатор (например, SROA) попытается заменить их быстрыми виртуальными регистрами.

Зачем нужна оптимизация кода в компиляторе, и какие основные цели она преследует?
Ответ: Оптимизация нужна для того, чтобы сделать программу более эффективной без изменения её логики. Основными целями являются увеличение скорости выполнения кода, уменьшение размера исполняемого файла и снижение потребления энергии или памяти.

Что такое SSA-форма и почему она важна при оптимизации программ?
Ответ: SSA (Static Single Assignment) — это форма представления кода, в которой каждой переменной значение присваивается ровно один раз. Это критически важно для оптимизации, так как SSA-форма делает зависимости между данными явными, позволяя компилятору легко отслеживать, откуда пришло значение переменной в любой точке программы.

Что такое граф потока управления (CFG) и как он помогает анализировать поведение программы?
Ответ: Граф потока управления — это структура, где узлы являются базовыми блоками кода, а ребра показывают возможные переходы (прыжки) между ними. CFG помогает компилятору анализировать логику ветвлений, находить циклы, обнаруживать недостижимый код и эффективно распределять ресурсы процессора.

Как устроено представление арифметических операций в LLVM IR (например, умножение, сложение)?
Ответ: Арифметические операции представлены в виде трехадресного кода, где инструкция (например, add или mul) принимает два операнда и сохраняет результат в новую виртуальную переменную. Каждая такая операция типизирована, что исключает ошибки несоответствия типов данных на уровне промежуточного кода.

Почему функции в LLVM IR обычно представляют собой отдельные единицы анализа и оптимизации?
Ответ: Функции обладают естественными границами видимости и управления стеком, что позволяет компилятору эффективно обрабатывать их по отдельности, экономя время и память. Такой подход упрощает локальный анализ переменных и позволяет параллельно оптимизировать разные части программы.

Что происходит с функцией в LLVM IR, если она вызывается один раз и очень короткая?
Ответ: В таком случае компилятор обычно применяет инлайнинг (Inlining) — он вставляет тело функции напрямую в место вызова. Это убирает накладные расходы на вызов функции (подготовку стека и передачу аргументов), что значительно ускоряет выполнение программы.

Какие преимущества даёт использование IR и CFG для автоматических оптимизаций по сравнению с анализом исходного текста на C?
Ответ: IR и CFG предоставляют «очищенную» от синтаксического шума структуру, где все зависимости и пути выполнения представлены в явном математическом виде. Это позволяет автоматическим алгоритмам работать быстрее и точнее, так как им не нужно разбираться в сложностях макросов, шаблонов или неоднозначностей исходного текста на C.
