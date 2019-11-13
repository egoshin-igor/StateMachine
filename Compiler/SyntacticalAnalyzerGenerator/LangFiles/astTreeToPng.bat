set argC=0
for %%x in (%*) do Set /A argC+=1

if %argC% == 0 goto basePathNotFound
set baseUrl=%1
cd "%baseUrl%"

:basePathNotFound
dot -Tpng astTree.dot -o astTree.png