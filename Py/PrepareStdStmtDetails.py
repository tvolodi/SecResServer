from sqlalchemy import  create_engine, func
from sqlalchemy.orm import  sessionmaker
from sqlalchemy.sql import select


from ModelDeclaration import PeriodType,\
                            SimFinEntity, \
                            SimFinIndustry, \
                            SimFinCompany, \
                            SimFinStmtRegistry, \
                            SimFinStdStmt, \
                            SimFinStdStmtDetail

import ModelDeclaration

dbEngine = create_engine('postgresql://SecResUser:VolWork1@localhost:5433/SecResServer')

dbConnection = dbEngine.connect()

Session = sessionmaker(bind=dbEngine)

session = Session()

simFinEntities = session.query(SimFinEntity).filter(SimFinEntity.Ticker == 'DOW').all()
for simFinEntity in simFinEntities:

    # session2 = Session()
    simFinStmtRegistries = session.query(SimFinStmtRegistry).filter(SimFinStmtRegistry.SimFinEntityId == simFinEntity.Id).all()
    for simFinStmtRegistry in simFinStmtRegistries:

        session3 = Session()
        simFinStdStmts = session3.query(SimFinStdStmt).filter(SimFinStdStmt.SimFinStmtRegistryId == simFinStmtRegistry.Id).all()
        for simFinStdStmt in simFinStdStmts:

            session4 = Session()
            simFinStmtDetails = session4.query(SimFinStdStmtDetail).filter(SimFinStdStmtDetail.SimFinStdStmtId == simFinStdStmt.Id).all()
            for simFinStmtDetail in simFinStmtDetails:
                print(simFinStmtDetail.StmtDetailName.Name)



print('Done')