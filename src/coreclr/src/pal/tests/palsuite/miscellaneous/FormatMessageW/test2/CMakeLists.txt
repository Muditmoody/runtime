set(SOURCES
  test.cpp
)

add_executable(paltest_formatmessagew_test2
  ${SOURCES}
)

add_dependencies(paltest_formatmessagew_test2 coreclrpal)

target_link_libraries(paltest_formatmessagew_test2
  ${COMMON_TEST_LIBRARIES}
)
