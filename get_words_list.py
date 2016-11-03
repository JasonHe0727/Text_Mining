import jieba

def removeStopWords(wordsSet,fileName):
	fileReader = open(fileName,"r")
	content = fileReader.read()
	wordsList = list(wordsSet)
	for word in wordsList:
		if content.find(word) >= 0:
			wordsSet.remove(word)
			print("{0} has been removed".format(word))

fileReader = open("/home/jasonhe/Data/auto sentiment训练集.csv","r")
trainData = []
firstLine = True
for line in fileReader:
	if firstLine:
		firstLine = False
	else:
		t = line.split('\t')
		item = t[0].lower()
		if item == 'positive':
			trainData.append([True,t[1].rstrip('\n')])
		elif item == 'negative':
			trainData.append([False,t[1].rstrip('\n')])
		else:
			print('drop 1 line')
# Positive: True Negative: False
fileReader.close()
seg_data = []
for i in trainData:
	seg_data.append(list(jieba.cut(i[1],cut_all = True)))
wordsSet = set()
for line in seg_data:
	for word in line:
		wordsSet.add(word)

removeStopWords(wordsSet,'/home/jasonhe/Data/stopwords.txt')

fileWriter = open("/home/jasonhe/Data/words.txt","w")
for word in wordsSet:
	fileWriter.write(word)
	fileWriter.write('\n')
fileWriter.close()
print('Finished') 
