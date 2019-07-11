from sqlalchemy import  create_engine
from sqlalchemy.orm import  sessionmaker

from ModelDeclaration import SimFinEntity, SimFinIndustry

import ModelDeclaration

dbEngine = create_engine('postgresql://SecResUser:VolWork1@localhost:5433/SecResServer')

Session = sessionmaker(bind=dbEngine)

session = Session()

simFinEntities = session.query(SimFinIndustry).all()

print("the end")


