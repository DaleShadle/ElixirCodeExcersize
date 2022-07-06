Design Intent
	o  To allow a user enter their id (Name), and provide answers to a set of at least 3 security questions out of 10 possible questions.
	o  To allow the user to provide an answer to any their previously entered security questions responding with success upon any one being correct.
	o  To allow a user who has forgotten their security answers to choose not to answer and be able to answer 3 new questions. 
	o  To allow for multiple users to be entered.
	o  To persist this information in a local file.  

Design Choices:
	In-Scope
		o  Storing of id to the question along with the provided answer.  Adding small amount of security by not having the question itself and answer in the same file.  
		o  Basic validation
		o  Persisted file is located in the project folder

	Outof Scope
		o  Maintenance (Allowing user to modify existing or change to new security answers).  Note:  Per requirements a user can enter their name and answer 
			no to entering questions which allows them to replace previous answers.  This req doesn't really make sense as a user can replace their previous answers
			without first answering at least 1. 
		o  Extensive validation
			o  Length check min,max, invalid characters for xml...
			o  Ensuring disk space avail/issue prevention
			o  Duplicate User names
			o  Edge break cases of keystrokes/actions/special characters
		o  File size/Cleanup
		o  Auditing/Logging
		o  Assumes small scale/sample i.e. loads entire file		

Other Considerations
	o  Given a proper security file the answers the questios would best be hashed.  Possibly the file itself encrypted.

