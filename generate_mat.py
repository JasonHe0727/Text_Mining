def findWords(sentence, words):
	result = []
	for i in range(len(words)):
		if sentence.find(words[i])>=0:
			result.append(i)
	return result

def getWords(filePath):
	fileReader = open(filePath,'r')
	words = []
	for line in fileReader:
		words.append(line.rstrip('\n'))
	return words

fileReader = open("/home/jasonhe/Data/auto sentiment训练集.csv",'r')
words = getWords("/home/jasonhe/Data/result_words.txt")
fileWriter = open("/home/jasonhe/Data/sparse_mat.txt",'w')
firstLine = True
lineNumber = 0
for line in fileReader:
	if firstLine:
		firstLine = False
	else:
		t = line.split('\t')
		item = t[0].lower()
		if item == 'positive':
			colIndexes = findWords(t[1],words)
			if len(colIndexes) >= 0:
				for i in colIndexes:
					fileWriter.write("{0}\t{1}\t1\n".format(lineNumber,i))
			fileWriter.write("{0}\t{1}\t1\n".format(lineNumber,len(words)))
		elif item == 'negative':
			colIndexes = findWords(t[1],words)
			if len(colIndexes) >= 0:
				for i in colIndexes:
					fileWriter.write("{0}\t{1}\t1\n".format(lineNumber,i))
			fileWriter.write("{0}\t{1}\t0\n".format(lineNumber,len(words)))
		else:
			print("drop 1 line")
		lineNumber += 1
fileReader.close()
fileWriter.close()
print("finished!")
