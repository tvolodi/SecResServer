from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy import ForeignKey
from sqlalchemy.orm import relationship

from sqlalchemy import Column, Integer, String, Boolean, DateTime, Float

Base = declarative_base()


class StmtType(Base):
    __tablename__ = 'StmtTypes'
    Id = Column(Integer, primary_key = True)
    Name = Column(String)


class StmtDetailName(Base):
    __tablename__ = 'StmtDetailNames'
    Id = Column(Integer, primary_key=True)
    Name = Column(String)


class PeriodType(Base):
    __tablename__ = 'PeriodTypes'
    Id = Column(Integer, primary_key=True)
    Name = Column(String)


class Currency(Base):
    __tablename__ = 'Currencies'
    Id = Column(Integer, primary_key=True)
    CharCode = Column(String)
    Name = Column(String)


class SimFinSector(Base):
    __tablename__ = 'SimFinSectors'
    Id = Column(Integer, primary_key = True)
    Code = Column(Integer)
    Name = Column(String)


class SimFinStmtIndustryTemplate(Base):
    __tablename__ = 'SimFinStmtIndustryTemplates'
    Id = Column(Integer, primary_key=True)
    Name = Column(String)


# Entity of SimFin portal
class SimFinEntity(Base):
    __tablename__ = 'SimFinEntities'
    Id = Column(Integer, primary_key = True)
    SimFinId = Column(Integer)
    Ticker = Column(String)
    Name = Column(String)
    IsStmtRegistryLoaded = Column(Boolean)
    LastUpdateDT = Column(DateTime)
    DeleteDT = Column(DateTime)


class SimFinIndustry(Base):
    __tablename__ = 'SimFinIndustries'
    Id = Column(Integer, primary_key = True)
    Code = Column(Integer)
    Name = Column(String)
    SimFinSectorId = Column(Integer, ForeignKey(SimFinSector.Id))
    SimFinSector = relationship("SimFinSector") # , foreign_keys = 'SimFinSector.Id')


class SimFinCompany(Base):
    __tablename__ = 'SimFinCompanies'
    Id = Column(Integer, primary_key = True)
    SimFinEntityId = Column(Integer, ForeignKey(SimFinEntity.Id))
    SimFinEntity = relationship("SimFinEntity")
    Name = Column(String)
    PYearEnd = Column(Integer)
    EmployeesQnt = Column(Integer)
    SimFinIndustryId = Column(Integer, ForeignKey(SimFinIndustry.Id))
    SimFinIndustry = relationship("SimFinIndustry")


class SimFinStmtRegistry(Base):
    __tablename__ = 'SimFinStmtRegistries'
    Id = Column(Integer, primary_key=True)
    SimFinEntityId = Column(Integer, ForeignKey(SimFinEntity.Id))
    SimFinEntity = relationship("SimFinEntity")
    StmtTypeId = Column(Integer, ForeignKey(StmtType.Id))
    StmtType = relationship("StmtType")
    FYear = Column(Integer)
    PeriodTypeId = Column(Integer, ForeignKey(PeriodType.Id))
    PeriodType = relationship("PeriodType")
    LoadDateTime = Column(DateTime)
    OrigStmtLoadDT = Column(DateTime)
    StdStmtLoadDT = Column(DateTime)
    IsCalculated = Column(Boolean)


class SimFinOriginalStmt(Base):
    __tablename__ = 'SimFinOriginalStmts'
    Id = Column(Integer, primary_key=True)
    SimFinStmtRegistryId = Column(Integer, ForeignKey(SimFinStmtRegistry.Id))
    SimFinStmtRegistry = relationship("SimFinStmtRegistry")
    PeriodEndDate = Column(DateTime)
    FirstPublishedDate = Column(DateTime)
    CurrencyId = Column(Integer, ForeignKey(Currency.Id))
    Currency = relationship("Currency")
    PeriodTypeId = Column(Integer, ForeignKey(PeriodType.Id))
    PeriodType = relationship("PeriodType")
    FYear = Column(Integer)
    IsStmtDetailsLoaded = Column(Boolean)


class SimFinOrigStmtDetail(Base):
    __tablename__ = 'SimFinOrigStmtDetails'
    Id = Column(Integer, primary_key=True)
    LineItemId = Column(Integer)
    StmtDetailNameId = Column(Integer, ForeignKey(StmtDetailName.Id))
    StmtDetailName = relationship("StmtDetailName")
    SimFinOriginalStmtId = Column(Integer, ForeignKey(SimFinOriginalStmt.Id))
    SimFinOriginalStmt = relationship("SimFinOriginalStmt")
    Value = Column(Float)


class SimFinStdStmt(Base):
    __tablename__ = 'SimFinStdStmts'
    Id = Column(Integer, primary_key=True)
    SimFinStmtRegistryId = Column(Integer, ForeignKey(SimFinStmtRegistry.Id))
    SimFinStmtRegistry = relationship("SimFinStmtRegistry")
    SimFinStmtIndustryTemplateId = Column(Integer, ForeignKey(SimFinStmtIndustryTemplate.Id))
    SimFinStmtIndustryTemplate = relationship("SimFinStmtIndustryTemplate")
    FYear = Column(Integer)
    PeriodTypeId = Column(Integer, ForeignKey(PeriodType.Id))
    PeriodType = relationship('PeriodType')
    PeriodEndDate = Column(DateTime)
    IsStmtDetailsLoaded = Column(Boolean)


class SimFinStdStmtDetail(Base):
    __tablename__ = 'SimFinStdStmtDetails'
    Id = Column(Integer, primary_key=True)
    SimFinStdStmtId = Column(Integer, ForeignKey(SimFinStdStmt.Id))
    SimFinStdStmt = relationship('SimFinStdStmt')
    StmtDetailNameId = Column(Integer, ForeignKey(StmtDetailName.Id))
    StmtDetailName = relationship('StmtDetailName')
    TId = Column(Integer)
    UId = Column(Integer)
    DisplayLevel = Column(Integer)
    ParentTId = Column(Integer)
    ValueAssigned = Column(Float)
    ValueCalculated = Column(Float)
    ValueChosen = Column(Float)



