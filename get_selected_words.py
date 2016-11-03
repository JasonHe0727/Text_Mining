fileReader = open('/home/jasonhe/Data/selected_words.txt','r')
k = 500
word_indexes = []
index = 0
for line in fileReader:
	if index < k:
		word_indexes.append(int(line.lstrip('(').split(',')[0]))
	else:
		break
	index += 1
fileReader.close()
selected_words = []
fileReader = open('/home/jasonhe/Data/words.txt','r')
index = 0
for line in fileReader:
	if index in word_indexes:
		selected_words.append(line.rstrip('\n'))
	index += 1
for word in selected_words:
	print(word)
print(len(selected_words))
