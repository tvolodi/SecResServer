from sqlalchemy import  create_engine
from sqlalchemy.orm import  sessionmaker
from sqlalchemy.sql import select

from ModelDeclaration import SimFinEntity, SimFinIndustry, SimFinCompany, SimFinStmtRegistry

import ModelDeclaration

dbEngine = create_engine('postgresql://SecResUser:VolWork1@localhost:5433/SecResServer')

dbConnection = dbEngine.connect()

Session = sessionmaker(bind=dbEngine)

session = Session()

simFinEntities = session.query(SimFinEntity).filter(SimFinEntity.Ticker == 'DOW').all()

simFinEntities2 = dbConnection.execute(select([SimFinEntity]))
for sim_fin_entity in simFinEntities2:
    # get stmt registry for the SimFinEntity
    sim_fin_stmt_registry = db.


selectionResult = dbConnection.execute(selection)


print('Done')